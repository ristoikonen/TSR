namespace TSR.Web;

using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

public class HashApi()
{

    //perceptual hash 
    public async Task<ulong> GetHash(byte[] data, CancellationToken cancellationToken = default)
    {
        ulong imageHash = 0;
        // Choose a hashing algorithm
        var hashAlgorithm = new PerceptualHash(); // Or DifferenceHash, AverageHash
        

        // Load the image using ImageSharp
        using var image = Image.Load<Rgba32>(data);

        if (image is not null)
        {
            imageHash = hashAlgorithm.Hash(image);
        }

        
        return imageHash;
    }


    public async Task<double> CompareHashes(byte[] data, byte[] data2)
    {
        return CompareHash.Similarity(data, data2);
    }

    public async Task<double> CompareHashes(ulong data, ulong data2)
    {
        return CompareHash.Similarity(data, data2);
    }

    public async Task<string> CompareHashesPercent(ulong data, ulong data2, string format)
    {
        return CompareHash.Similarity(data, data2).ToString(format);
    }
}

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast", cancellationToken))
        {
            if (forecasts?.Count >= maxItems)
            {
                break;
            }
            if (forecast is not null)
            {
                forecasts ??= [];
                forecasts.Add(forecast);
            }
        }

        return forecasts?.ToArray() ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
