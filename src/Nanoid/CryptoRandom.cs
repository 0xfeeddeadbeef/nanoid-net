using System;
using System.Security.Cryptography;

namespace NanoidDotNet;

/// <summary>
/// Implementation of <see cref="Random"/> using <see cref="RandomNumberGenerator"/>.
/// </summary>
public class CryptoRandom : Random
{
#if !NET
    private readonly RandomNumberGenerator _r;
    private readonly byte[] _uint32Buffer = new byte[4];
#endif

    /// <inheritdoc/>
    public CryptoRandom()
    {
#if !NET
        _r = RandomNumberGenerator.Create();
#endif
    }

    /// <inheritdoc/>
    public override void NextBytes(byte[] buffer)
    {
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
#if NET
        RandomNumberGenerator.Fill(buffer.AsSpan());
#else
        _r.GetBytes(buffer);
#endif
    }

#if NET
    /// <inheritdoc/>
    public override void NextBytes(Span<byte> buffer)
    {
        RandomNumberGenerator.Fill(buffer);
    }
#endif

    /// <inheritdoc/>
    public override double NextDouble()
    {
#if NET
        Span<byte> uint32Buffer = stackalloc byte[4];
        RandomNumberGenerator.Fill(uint32Buffer);
        return BitConverter.ToUInt32(uint32Buffer) / (1.0 + UInt32.MaxValue);
#else
        _r.GetBytes(_uint32Buffer);
        return BitConverter.ToUInt32(_uint32Buffer, 0) / (1.0 + UInt32.MaxValue);
#endif
    }

    /// <inheritdoc/>
    public override int Next(int minValue, int maxValue)
    {
        if (minValue > maxValue) throw new ArgumentOutOfRangeException(nameof(minValue));
        if (minValue == maxValue) return minValue;
        var range = (long)maxValue - minValue;
        return (int)((long)Math.Floor(NextDouble() * range) + minValue);
    }

    /// <inheritdoc/>
    public override int Next()
    {
        return Next(0, int.MaxValue);
    }

    /// <inheritdoc/>
    public override int Next(int maxValue)
    {
        if (maxValue < 0) throw new ArgumentOutOfRangeException(nameof(maxValue));
        return Next(0, maxValue);
    }
}
