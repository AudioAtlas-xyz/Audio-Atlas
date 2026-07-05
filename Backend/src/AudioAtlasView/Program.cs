using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AudioAtlasView;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasApplication.Services.Dashboard;
using AudioAtlasInfrastructure.Repositories;
using AudioAtlasInfrastructure.Services;
using AudioAtlasInfrastructure.Services.Dashboard;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;  
using System.Text.Json;
[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Environment.EnvironmentName);

builder.Configuration.AddJsonFile(
    $"appsettings.{builder.Environment.EnvironmentName}.Json",
    optional: true,
    reloadOnChange: true);

if (builder.Environment.IsProduction())
{
    builder.Configuration
        .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Production.Json", optional: true, reloadOnChange: true);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//Dependency injection HAS TO be here, or else mapping of controllers will crash.
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IInstrumentRepository, InstrumentRepository>();
builder.Services.AddScoped<IUserDeletionService, UserDeletionService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<ISearchQueryRepository, SearchQueryRepository>();
builder.Services.AddScoped<ICatalogueQueryService, CatalogueQueryService>();
builder.Services.AddScoped<IPipelineQueryService, PipelineQueryService>();
builder.Services.AddScoped<ICommunityQueryService, CommunityQueryService>();
builder.Services.AddScoped<IDiscoveryQueryService, DiscoveryQueryService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Select(origin => origin.Trim().TrimEnd('/'))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray() ?? throw new Exception("CORS AllowedOrigins not configured");

if (allowedOrigins.Length == 0)
{
    throw new Exception(
        $"CORS AllowedOrigins is empty for environment '{builder.Environment.EnvironmentName}'. " +
        "Check that the production settings file is deployed with the correct Linux casing.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

var key = Encoding.UTF8.GetBytes(jwtKey);

var githubClientId = builder.Configuration["Authentication:GitHub:ClientId"]
    ?? throw new InvalidOperationException("Authentication:GitHub:ClientId is not configured.");
var githubClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]
    ?? throw new InvalidOperationException("Authentication:GitHub:ClientSecret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            var stampClaim = ctx.Principal?.FindFirst("stamp")?.Value;

            // Tokens issued before this fix carry no stamp claim. Allow them
            // through — they will expire naturally within one hour. Once every
            // token in circulation was issued after this deploy, any token
            // missing the stamp claim will be rejected automatically.
            if (string.IsNullOrEmpty(stampClaim))
                return;

            var userId = ctx.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) { ctx.Fail("Invalid token."); return; }

            try
            {
                var userManager = ctx.HttpContext.RequestServices
                    .GetRequiredService<UserManager<ApplicationUser>>();

                var user = await userManager.FindByIdAsync(userId);

                if (user == null || user.SecurityStamp != stampClaim)
                    ctx.Fail("Token has been revoked.");
            }
            catch
            {
                ctx.Fail("Could not validate token.");
            }
        }
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;

    options.CallbackPath = "/signin-google";

    options.Scope.Add("email");
    options.Scope.Add("profile");

    options.SignInScheme = IdentityConstants.ExternalScheme;
})
.AddGitHub(options =>
{
    options.ClientId = githubClientId;
    options.ClientSecret = githubClientSecret;

    options.CallbackPath = "/signin-github";

    options.Scope.Add("user:email");

    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.SaveTokens = true;

    options.Events.OnCreatingTicket = async context =>
    {
        if (!context.Identity!.HasClaim(claim => claim.Type == ClaimTypes.Email))
        {
            var emailRequest = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
            emailRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
            emailRequest.Headers.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            emailRequest.Headers.UserAgent.ParseAdd("AudioAtlas");

            var emailResponse = await context.Backchannel.SendAsync(emailRequest);
            emailResponse.EnsureSuccessStatusCode();

            using var emailDocument = JsonDocument.Parse(await emailResponse.Content.ReadAsStringAsync());
            var primaryVerifiedEmail = emailDocument.RootElement
                .EnumerateArray()
                .FirstOrDefault(email =>
                    email.TryGetProperty("primary", out var primary) &&
                    primary.GetBoolean() &&
                    email.TryGetProperty("verified", out var verified) &&
                    verified.GetBoolean() &&
                    email.TryGetProperty("email", out _));

            if (primaryVerifiedEmail.ValueKind != JsonValueKind.Undefined &&
                primaryVerifiedEmail.TryGetProperty("email", out var emailValue))
            {
                context.Identity.AddClaim(new Claim(ClaimTypes.Email, emailValue.GetString()!));
            }
        }
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var ctx = services.GetRequiredService<AppDbContext>();
    var seedLogger = services.GetRequiredService<ILogger<DbInitializer>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    app.Logger.LogInformation("Running database migration and seed.");

    ctx.Database.Migrate();

    await DbInitializer.SeedDatabase(ctx, userManager, roleManager, builder.Configuration, seedLogger);

    app.Logger.LogInformation("Database migration and seed completed.");
    ViewDebugger.DebugToFile(ctx);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

//app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
