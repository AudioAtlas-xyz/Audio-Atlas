using AudioAtlasInfrastructure.Database;
using AudioAtlasInfrastructure.Database.Seed;
using AudioAtlasDomain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = builder.Environment.IsDevelopment() 
        ? AppDbContextDefaults.DevelopmentConnectionString 
        : throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Set ConnectionStrings:DefaultConnection before starting the app.");
}

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


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    /*var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var seedLogger = scope.ServiceProvider.GetRequiredService<ILogger<DbInitializer>>();

    app.Logger.LogInformation("Running database migration and seed.");
    //ctx.Database.Migrate(); - does not work on Ubuntu :(
    DbInitializer.SeedDatabase(ctx, seedLogger);
    app.Logger.LogInformation("Database migration and seed completed.");
    */
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
app.MapGet("/", () => "Hello World");
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

