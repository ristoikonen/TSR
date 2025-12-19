using System;
using System.Diagnostics.CodeAnalysis;

namespace TSR.ApiService.Services;


// was CompareHash https://github.com/coenm/ImageHash/blob/develop/src/ImageHash/CompareHash.cs#L41

/// <summary>
/// Utility to compare 64 bit hashes using the Hamming distance.
/// </summary>
public static class ImageCompareHash
{
    /// <summary>
    /// Array used for BitCount method (used in Similarity comparisons).
    /// Array corresponds to the number of 1's per value.
    /// ie. index 0 => byte 0x00 = 0000 0000 -> 0 high bits
    /// ie. index 1 => byte 0x01 = 0000 0001 -> 1 high bit
    /// ie. index 3 => byte 0x03 = 0000 0011 -> 2 high bits
    /// ie. index 255 =>    0xFF = 1111 1111 -> 8 high bits
    /// etc.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1001:CommasMustBeSpacedCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    private static readonly byte[] _bitCounts =
    {
            0,1,1,2,1,2,2,3, 1,2,2,3,2,3,3,4, 1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5,
            1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
            1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
            2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
            1,2,2,3,2,3,3,4, 2,3,3,4,3,4,4,5, 2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6,
            2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
            2,3,3,4,3,4,4,5, 3,4,4,5,4,5,5,6, 3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7,
            3,4,4,5,4,5,5,6, 4,5,5,6,5,6,6,7, 4,5,5,6,5,6,6,7, 5,6,6,7,6,7,7,8,
    };

    /// <summary>
    /// Returns a percentage-based similarity value between the two given hashes. The higher
    /// the percentage, the closer the hashes are to being identical.
    /// </summary>
    /// <param name="hash1">The first hash.</param>
    /// <param name="hash2">The second hash.</param>
    /// <returns>The similarity percentage.</returns>
    public static double Similarity(ulong hash1, ulong hash2)
    {
        var similar = (64 - BitCount(hash1 ^ hash2)) * 100 / 64.0;
        return similar;
    }

    /// <summary>
    /// Returns a percentage-based similarity value between the two given hashes. The higher
    /// the percentage, the closer the hashes are to being identical.
    /// </summary>
    /// <param name="hash1">The first hash. Cannot be null and must have a length of 8.</param>
    /// <param name="hash2">The second hash. Cannot be null and must have a length of 8.</param>
    /// <returns>The similarity percentage.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="hash1"/> or <paramref name="hash2"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="hash1"/> or <paramref name="hash2"/> has a length other than <c>8</c>.</exception>
    public static double Similarity(byte[] hash1, byte[] hash2)
    {
        if (hash1 == null)
        {
            throw new ArgumentNullException(nameof(hash1));
        }

        if (hash2 == null)
        {
            throw new ArgumentNullException(nameof(hash2));
        }

        if (hash1.Length != 8)
        {
            throw new ArgumentOutOfRangeException(nameof(hash1));
        }

        if (hash2.Length != 8)
        {
            throw new ArgumentOutOfRangeException(nameof(hash2));
        }

        var h1 = BitConverter.ToUInt64(hash1, 0);
        var h2 = BitConverter.ToUInt64(hash2, 0);
        return Similarity(h1, h2);
    }


    public static double Similarity(float[] hash1, float[] hash2)
    {
        if (hash1 == null) throw new ArgumentNullException(nameof(hash1));
        if (hash2 == null) throw new ArgumentNullException(nameof(hash2));
        if (hash1.Length != hash2.Length) throw new ArgumentException("Hash arrays must have the same length.", nameof(hash2));

        int len = hash1.Length;
        if (len == 0) return 100.0;

        double dot = 0.0, normA = 0.0, normB = 0.0;
        for (int i = 0; i < len; i++)
        {
            double a = hash1[i];
            double b = hash2[i];
            dot += a * b;
            normA += a * a;
            normB += b * b;
        }

        if (normA == 0.0 || normB == 0.0) return 0.0;

        double cos = dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        cos = Math.Max(-1.0, Math.Min(1.0, cos)); // clamp
        return ((cos + 1.0) / 2.0) * 100.0; // scale to 0..100
    }


    public static double Similarity(ulong[] hash1, ulong[] hash2)
    {
        if (hash1 == null)
        {
            throw new ArgumentNullException(nameof(hash1));
        }

        if (hash2 == null)
        {
            throw new ArgumentNullException(nameof(hash2));
        }

        return Similarity(hash1, hash2);
    }

    /// <summary>Counts bits Utility function for similarity.</summary>
    /// <param name="num">The hash we are counting.</param>
    /// <returns>The total bit count.</returns>
    private static uint BitCount(ulong num)
    {
        uint count = 0;
        for (; num > 0; num >>= 8)
        {
            count += _bitCounts[num & 0xff];
        }

        return count;
    }
}
