using Microsoft.Extensions.VectorData;

namespace TSR.ApiService.Models;

internal class TSRImage
{
    /// <summary>A unique key for the image.</summary>
    [VectorStoreKey]
    public required string FileName { get; init; }

    /// <summary>The embedding generated from the Image.</summary>
    [VectorStoreVector(2048)]
    public ReadOnlyMemory<float> ImageEmbedding { get; set; }
}
