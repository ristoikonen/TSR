using Microsoft.Extensions.VectorData;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OllamaSharp.Models;
//using OpenAI.VectorStores;
using System.Text.Json;
using TSR.ApiService;
using TSR.ApiService.Models;
using TSR.ApiService.Services;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAntiforgery();

builder.Services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

////if (!builder.Environment.IsDevelopment())
////{
////    builder.Services.AddAntiforgery(o =>
////    {
////        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
////    });
////}



// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInMemoryVectorStore();

// Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddSwaggerGen(options =>
//{
//    // Basic API info
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "Minimal API Example",
//        Version = "v1",
//        Description = "A simple Minimal API with Swagger documentation",
//        Contact = new OpenApiContact
//        {
//            Name = "API Support",
//            Email = "support@example.com"
//        }
//    });
//});



//// Enable Swagger in Development (or always if you want)
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Minimal API v1");
//        options.RoutePrefix = string.Empty; // Swagger at root URL
//    });
//}



var app = builder.Build();

app.UseAntiforgery();

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// 1. Register the Store as a Singleton
// This ensures the data persists as long as the app is running
//builder.Services.AddSingleton<VectorStore, InMemoryVectorStore>();

//// 2. Register your specific collection (optional but recommended)
//builder.Services.AddSingleton(sp =>
//{
//    var store = sp.GetRequiredService<VectorStore>();
//    return store.GetCollection<string, MyMemoryRecord>("user_memories");
//});

//var vectorStore = new InMemoryVectorStore();
// https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/vector-search?pivots=programming-language-csharp
//https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/embedding-generation?source=recommendations&pivots=programming-language-csharp


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "API service is running. use it by inserting Au postcode as parameter to  to /postcode/2300 to test postcode 2300  .");

// /postcode/2914
app.MapGet("/postcode/{code}", async (string code, HttpResponse response) =>
{
    string jsonContent = File.ReadAllText("postcodes.json");
    if (!string.IsNullOrWhiteSpace(jsonContent) && !string.IsNullOrWhiteSpace(code))
    {
        AustralianPostcode[] postcodes = JsonSerializer.Deserialize<AustralianPostcode[]>(jsonContent) ?? Array.Empty<AustralianPostcode>();
        if (postcodes is null || postcodes is not AustralianPostcode[] || postcodes.Length < 1)
        {
            return Results.Json(null);
        }

        var postcode = postcodes.Where(p => p.Postcode == code).FirstOrDefault();
        //TODO: ifs
        string jsonString = JsonSerializer.Serialize(postcode, new JsonSerializerOptions { WriteIndented = true });
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //TODO: Add appFolderName
        string fileName = code + "_" + Guid.NewGuid().ToString() + ".json";
        string fullPath = Path.Combine(documentsPath, fileName);
        //TODO: log this
        File.WriteAllText(fullPath, jsonString);

        return Results.Json(postcode ?? null);
    }
    
    return Results.Json(string.Empty);
});

//app.MapGet("/vector", () =>
//{
//    return Results.Json(new { message = "Vector endpoint placeholder" });
//});


// Accept an image upload (multipart/form-data file field or raw body bytes) and return its embedding vector
app.MapPost("/vector", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
{
    byte[] imageBytes;
    string fileName = "upload";

    var fileform = request.Form.Files[0];

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

    var generator = new ImageVectorGenerator();
    var vector = await generator.GenerateVectorFromImage(imageBytes, fileName);

    return Results.Json(new { vector });
});




// Accept an image upload (multipart/form-data file field or raw body bytes) and return its embedding vector
app.MapPost("/image", async (IFormFile file) =>
{
    var fileName = file.Name;
    byte[] imageBytes;
    if (file == null || file.Length == 0) 
        return Results.BadRequest("File empty");

    var path = Path.Combine("Uploads", file.FileName);
    //file.CopyToAsync(new FileStream(path, FileMode.Create).Read(imageBytes).;
    using (var fs = new FileStream(path, FileMode.Create))
    {
        //await fs.CopyToAsync(stream);
        //imageBytes = file.ToArray();
        fs.Seek(0, SeekOrigin.Begin);

        // Use the 'using' statement to ensure MemoryStream is disposed
        using (MemoryStream ms = new MemoryStream())
        {
            // Copy the entire contents of the FileStream to the MemoryStream
            await fs.CopyToAsync(ms);

            // Return the byte array from the MemoryStream
            imageBytes = ms.ToArray();
        }
        //stream.Read(imageBytes = new byte[file.Length], 0, (int)file.Length);
        fs.Close();
    }



    var generator = new ImageVectorGenerator();
    var vector = await generator.GenerateVectorFromImage(imageBytes, fileName);

    return Results.Json(new { vector });
});


// Accept an image upload (multipart/form-data file field or raw body bytes) and return its embedding vector

//app.MapPost("/file2vector", async (Microsoft.AspNetCore.Http.HttpRequest request) =>
app.MapPost("/file2vector", async (IFormFile file) =>
{
    var fileName = Path.GetFileName(file.Name);
    byte[] imageBytes;

    if (file == null || file.Length == 0)
        return Results.BadRequest("File empty");



    //string fileName = "upload";
    ImageVectorGenerator image_vector_generator = new ImageVectorGenerator();

    // Try to bind a local file path from query string first
    //var query = request.Query;
    string? filePath = fileName;// query.ContainsKey("filePath") ? query["filePath"].ToString() : null;
    return Results.Json(filePath ?? "no filepath");

    //if (!string.IsNullOrWhiteSpace(filePath))
    //{
    //    if (!System.IO.File.Exists(filePath))
    //    {
    //        return Results.BadRequest($"File not found: {filePath}");
    //    }

    //    imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
    //    fileName = System.IO.Path.GetFileName(filePath);
    //    if (imageBytes is not null && imageBytes.Length > 0)
    //    {
    //        var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
    //        return Results.Json(new { vec });
    //    }
    //    return Results.BadRequest($"No data");
    //}
    //else if (request.HasFormContentType)
    //{
    //    var form = await request.ReadFormAsync();

    //    // Allow a form field named filePath as alternative
    //    if (string.IsNullOrWhiteSpace(filePath) && form.TryGetValue("filePath", out var fpValues))
    //    {
    //        var fp = fpValues.ToString();
    //        if (!string.IsNullOrWhiteSpace(fp) && System.IO.File.Exists(fp))
    //        {
    //            imageBytes = await System.IO.File.ReadAllBytesAsync(fp);
    //            fileName = System.IO.Path.GetFileName(fp);
    //            if (imageBytes is not null && imageBytes.Length > 0)
    //            {
    //                var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
    //                return Results.Json(new { vec });
    //            }
    //            return Results.BadRequest($"No data");
    //        }
    //        else
    //        {
    //            return Results.BadRequest($"File not found: {fp}");
    //        }
    //    }


    //    else if (form.Files.Count > 0)
    //    {
    //        var file = form.Files[0];
    //        fileName = string.IsNullOrWhiteSpace(file.FileName) ? fileName : file.FileName;
    //        using var ms = new System.IO.MemoryStream();
    //        await file.CopyToAsync(ms);
    //        if (ms is not null && ms.Length > 0)
    //        {
    //            imageBytes = ms.ToArray();
    //            var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
    //            return Results.Json(new { vec });
    //        }
    //        return Results.BadRequest($"No data");

    //    }
    //    else
    //    {
    //        // Fallback: read raw body bytes
    //        using var ms = new System.IO.MemoryStream();
    //        await request.Body.CopyToAsync(ms);
    //        imageBytes = ms.ToArray();
    //        if (imageBytes is not null && imageBytes.Length > 0)
    //        {
    //            var vec = await image_vector_generator.GenerateVectorFromImage(imageBytes, fileName);
    //            return Results.Json(new { vec });
    //        }
    //        return Results.BadRequest($"No data");
    //    }



});




//return Results.BadRequest("No file uploaded and no filePath provided.");

////if (imageBytes is null || imageBytes.Length == 0)
////{
////    return Results.BadRequest("Empty image payload.");
////}

//var generator = new TSR.ApiService.ImageVectorGenerator();
//var vector = await generator.GenerateVectorFromImage(imageBytes, fileName);

//return Results.Json("Empty image payload.");



app.MapDefaultEndpoints();

app.Run();


