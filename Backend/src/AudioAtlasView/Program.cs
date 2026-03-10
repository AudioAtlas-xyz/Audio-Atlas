var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/api/countries", () => "Heyerrbod!");

app.MapGet("/api/countries/{id}/genres", () => "Hello World!");

app.MapGet("/api/genres/{id}", () => "Hello Girl!");

app.Run();