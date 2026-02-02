using TSR.ApiService.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace TSR.ApiService.Services;

////BitmapMetadata thumbMeta = new BitmapMetadata("jpg");
////thumbMeta.Title = "thumbnail";
////JpegBitmapEncoder encoder = new JpegBitmapEncoder();
////encoder.QualityLevel = 100;
////encoder.Frames.Add(BitmapFrame.Create(tb, null, thumbMeta, null));
////using (FileStream stream = new FileStream(outputImage, FileMode.Create))
////{
////    encoder.Save(stream);
////    stream.Close();
////}

public static class ImageInfoReader
{

    public static ExifVector? GetExifVector(string fileNamepath)
    {
        if (File.Exists(fileNamepath))
        {
            using var image = Image.Load(fileNamepath);
            var exif_profile = image.Metadata.ExifProfile;
            if (exif_profile is not null)
            {
                // ExifVector? exif_vector
                return new ExifVector(exif_profile);
            }
        }
        return null;
    }

    public static SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile? GetExifProfile(string fileNamepath)
    {
        if (File.Exists(fileNamepath))
        {
            using var image = Image.Load(fileNamepath);
            return image.Metadata.ExifProfile;
        }
        return null;
    }
}




//if (exif_profile != null)
//{
//    // Access a specific tag directly
//    //var software = exif_profile.Values(ExifTag.Software);
//    //Console.WriteLine($"Software: {software?.Value}");
//    foreach (IExifValue value in exif_profile.Values)
//    {
//        var tag = value.Tag;
//        if (tag != null)
//        { 
//            tag.
//        }
//        // Tag describes what it is (e.g., "ISOSpeedRatings")
//        Console.WriteLine($"{value.Tag}: {value.GetValue()}");
//    }
//}

//public string Id { get; set; } = Guid.NewGuid().ToString();
//public string FileName { get; set; } = string.Empty;
//public float[] ImageEmbedding { get; set; } = Array.Empty<float>();
//public float[] GetVector()
//{
//    return ImageEmbedding;
//}
