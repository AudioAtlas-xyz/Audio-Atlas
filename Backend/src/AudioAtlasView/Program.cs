using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
