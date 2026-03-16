using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Environment.IsDevelopment()
        ? AppDbContextDefaults.DevelopmentConnectionString
        : throw new InvalidOperationException(
            "Connection string 'DefaultConnection' is not configured. Set ConnectionStrings:DefaultConnection before starting the app.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        }));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

    app.Logger.LogInformation("Running database migration and seed.");
    ctx.Database.Migrate();
    DbInitializer.SeedDatabase(ctx, seedLogger);
    app.Logger.LogInformation("Database migration and seed completed.");
}

app.Run();
