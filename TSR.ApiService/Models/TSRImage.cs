using Microsoft.Extensions.VectorData;

namespace TSR.ApiService.Models;

public class TSRImageExif //: ExifVector
{
    public TSRImageExif()
    {
        
    }

    /// <summary>A unique key for the image.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [VectorStoreKey]
    public required string FileName { get; init; } = string.Empty;

    /// <summary>The embedding generated from the Image.</summary>
    [VectorStoreVector(2048)]
    public ReadOnlyMemory<float> ImageEmbedding { get; set; } = Array.Empty<float>();

    [VectorStoreVector(2048)]
    public SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile? ExifProfile { get; set; }
}

public class TSRImage
{
    /// <summary>A unique key for the image.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [VectorStoreKey]
    public required string FileName { get; init; } = string.Empty;

    /// <summary>The embedding generated from the Image.</summary>
    [VectorStoreVector(2048)]
    public ReadOnlyMemory<float> ImageEmbedding { get; set; } = Array.Empty<float>();

}

public class ExifVector
{
    // Added a parameterless constructor so subclasses can be constructed without providing an ExifProfile.
    public ExifVector()
    {
    }

    public ExifVector(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile exifProfile)
    {
        this.ExifProfile = exifProfile;
    }

    [VectorStoreVector(2048)]
    public SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile? ExifProfile { get; set; }

    //public void SetExifProfile(SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile exifProfile)
    //{
    //    this.ExifProfile = exifProfile;
    //}

}

//public class TSRImage : IVectorData<string>
//{

//    public float[] ImageEmbedding { get; set; } = Array.Empty<float>();
//    public float[] GetVector()
//    {
//        return ImageEmbedding;
//    }
//}
