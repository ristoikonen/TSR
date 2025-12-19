using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp.PixelFormats;
using TSR.ApiService.Services;
using SixLabors.ImageSharp;

namespace TSR.Web.ApiClients;


public class HashApiClient()
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


    public async Task<double> CompareImagesHashes(byte[] data, byte[] data2)
    {
        return ImageCompareHash.Similarity(data, data2);
    }

    public async Task<double> CompareImagesHashes(ulong[] data, ulong[] data2)
    {
        return ImageCompareHash.Similarity(data, data2);
    }

    public async Task<string?> CompareImagesHashes(float[]? data, float[]? data2, string format)
    {
        if (data == null || data2 == null)
        {
            return null;
        }
        // format
        return ImageCompareHash.Similarity(data, data2).ToString();
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
