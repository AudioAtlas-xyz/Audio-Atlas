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
[assembly: ApiController]
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        }));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();



builder.Services.AddOpenApi();

//Dependency injection HAS TO be here, or else mapping of controllers will crash.
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IGenreService, GenreService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:3001",
                "http://127.0.0.1:3001",
                "http://localhost:3002",
                "http://127.0.0.1:3002"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("FrontendDev");
}
else
{
    app.UseHttpsRedirection();
}

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
