using System.Text.Json;
using TSR.ApiService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/", () => "API service is running. Navigate to /weatherforecast to see sample data.");

// /postcode/2914
app.MapGet("/postcode/{code}", async (string code, HttpResponse response) =>
{
    string jsonContent = File.ReadAllText("postcodes.json");
    if (!string.IsNullOrWhiteSpace(jsonContent) && !string.IsNullOrWhiteSpace(code))
    {
        AustralianPostcode[] postcodes = JsonSerializer.Deserialize<AustralianPostcode[]>(jsonContent) ?? Array.Empty<AustralianPostcode>();
        var postcode = postcodes.Where(p => p.Postcode == code).First();
        return Results.Json(postcode);
    }
    
    return Results.Json("");
});


app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
