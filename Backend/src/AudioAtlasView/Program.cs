using AudioAtlasInfrastructure.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
<<<<<<< HEAD
=======

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



>>>>>>> main
var app = builder.Build();

app.MapGet("/api/countries", () => "Heyerrbod!");

app.MapGet("/api/countries/{id}/genres", () => "Hello World!");

<<<<<<< HEAD
app.MapGet("/api/genres/{id}", () => "Hello Girl!");

app.Run();
=======

app.Run();
>>>>>>> main
