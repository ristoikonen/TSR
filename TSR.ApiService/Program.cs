using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp.Models;
using System.Text.Json;
using TSR.ApiService;
using TSR.ApiService.Models;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInMemoryVectorStore();

var app = builder.Build();

//var vectorStore = new InMemoryVectorStore();

// https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/vector-search?pivots=programming-language-csharp
//https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/embedding-generation?source=recommendations&pivots=programming-language-csharp


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

app.MapGet("/vector", () =>
{
    return Results.Json(new { message = "Vector endpoint placeholder" });
});



// Accept an image upload (multipart/form-data file field or raw body bytes) and return its embedding vector
app.MapPost("/vector", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
{
    byte[] imageBytes;
    string fileName = "upload";

    if (request.HasFormContentType)
    {
        var form = await request.ReadFormAsync();
        if (form.Files.Count == 0)
        {
            return Results.BadRequest("No file uploaded.");
        }

        var file = form.Files[0];
        fileName = string.IsNullOrWhiteSpace(file.FileName) ? fileName : file.FileName;
        using var ms = new System.IO.MemoryStream();
        await file.CopyToAsync(ms);
        imageBytes = ms.ToArray();
    }
    else
    {
        using var ms = new System.IO.MemoryStream();
        await request.Body.CopyToAsync(ms);
        imageBytes = ms.ToArray();
    }

    if (imageBytes == null || imageBytes.Length == 0)
    {
        return Results.BadRequest("Empty image payload.");
    }

    var generator = new TSR.ApiService.ImageVectorGenerator();
    var vector = await generator.GenerateVectorFromImage(imageBytes, fileName);

    return Results.Json(new { vector });
});



// Accept an image upload (multipart/form-data file field or raw body bytes) and return its embedding vector
/*
app.MapPost("/file2vector", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
{
    byte[] imageBytes;
    string fileName = "upload";
    ImageVectorGenerator image_vector_generator = new ImageVectorGenerator();

    // Try to bind a local file path from query string first
    var query = request.Query;
    string? filePath = query.ContainsKey("filePath") ? query["filePath"].ToString() : null;

    if (!string.IsNullOrWhiteSpace(filePath))
    {
        if (!System.IO.File.Exists(filePath))
        {
            return Results.BadRequest($"File not found: {filePath}");
        }

        imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        fileName = System.IO.Path.GetFileName(filePath);
        if (imageBytes is not null && imageBytes.Length > 0)
        {
            var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
        }
    }
    else if (request.HasFormContentType)
    {
        var form = await request.ReadFormAsync();

        // Allow a form field named filePath as alternative
        if (string.IsNullOrWhiteSpace(filePath) && form.TryGetValue("filePath", out var fpValues))
        {
            var fp = fpValues.ToString();
            if (!string.IsNullOrWhiteSpace(fp) && System.IO.File.Exists(fp))
            {
                imageBytes = await System.IO.File.ReadAllBytesAsync(fp);
                fileName = System.IO.Path.GetFileName(fp);
                if (imageBytes is not null && imageBytes.Length > 0)
                {
                    var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
                }
            }
            else
            {
                return Results.BadRequest($"File not found: {fp}");
            }
        }
        else if (form.Files.Count > 0)
        {
            var file = form.Files[0];
            fileName = string.IsNullOrWhiteSpace(file.FileName) ? fileName : file.FileName;
            using var ms = new System.IO.MemoryStream();
            await file.CopyToAsync(ms);
            if (ms is not null && ms.Length > 0)
            { 
                imageBytes = ms.ToArray();
                var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
            }

        }
        //else
        //{
        //    return Results.BadRequest("No file uploaded and no filePath provided.");
        //}

    // request has not .HasFormContentType)
    else
        {
        // Fallback: read raw body bytes
        using var ms = new System.IO.MemoryStream();
        await request.Body.CopyToAsync(ms);
        imageBytes = ms.ToArray();
        if (imageBytes is not null && imageBytes.Length > 0)
        {
            var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
            return Results.Json(new { vec });
        }
    }

    ////if (imageBytes is null || imageBytes.Length == 0)
    ////{
    ////    return Results.BadRequest("Empty image payload.");
    ////}

    //var generator = new TSR.ApiService.ImageVectorGenerator();
    //var vector = await generator.GenerateVectorFromImage(imageBytes, fileName);

    return Results.Json("Empty image payload.");
});
*/





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
