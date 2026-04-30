using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AudioAtlasView;
using AudioAtlasApplication.Repositories;
using AudioAtlasApplication.Services;
using AudioAtlasInfrastructure.Repositories;
using AudioAtlasInfrastructure.Services;
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

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString));

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
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IInstrumentRepository, InstrumentRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? throw new Exception("CORS AllowedOrigins not configured");

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
    ?? throw new Exception("JWT Key not configured");
    
var key = Encoding.UTF8.GetBytes(jwtKey);

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
.AddOAuth("GitHub", options =>
{
    options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;

    options.CallbackPath = "/signin-github";

    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
    options.UserInformationEndpoint = "https://api.github.com/user";

    options.Scope.Add("user:email");

    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.SaveTokens = true;

    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

    options.Events.OnCreatingTicket = async context =>
    {
        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);
        request.Headers.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.UserAgent.ParseAdd("AudioAtlas");

        var response = await context.Backchannel.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var user = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        context.RunClaimActions(user.RootElement);

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

    app.Logger.LogInformation("Running database migration and seed.");

    ctx.Database.Migrate();

    await DbInitializer.SeedDatabase(ctx, userManager, seedLogger);

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
