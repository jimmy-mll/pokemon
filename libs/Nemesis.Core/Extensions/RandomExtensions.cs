using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Nemesis.Core.Extensions;

/// <summary>
/// Extensions methods for <see cref="Random"/> class.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Creates a random <see cref="Vector2"/> that is greater than or equal
    /// to <paramref name="min"/> and less than <paramref name="max"/>.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>A random <see cref="Vector2"/> between <paramref name="min"/> and <paramref name="max"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NextVector2(this Random random, float min, float max) =>
        new(random.NextSingle() * (max - min) + min, random.NextSingle() * (max - min) + min);
    
    /// <summary>
    /// Creates a random <see cref="Vector2"/> inside the <paramref name="rectangle"/>.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <param name="rectangle">The <see cref="Rectangle"/> that the <see cref="Vector2"/> will be inside.</param>
    /// <returns>A random <see cref="Vector2"/> inside the <paramref name="rectangle"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 NextVector2(this Random random, in Rectangle rectangle) =>
        new(random.Next(rectangle.X, rectangle.X + rectangle.Width), random.Next(rectangle.Y, rectangle.Y + rectangle.Height));
    
    /// <summary>
    /// Creates a random <see cref="Color"/> that is greater than or equal to 0 and less than 255.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <returns>A random <see cref="Color"/> between 0 and 255.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color NextColor(this Random random) =>
        new(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));

    /// <summary>
    /// Fills the specified <paramref name="dest"/> with random values from the specified <paramref name="src"/>.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <param name="src">The values to use to populate the <paramref name="dest"/>.</param>
    /// <param name="dest">The destination to fill with random values from the specified <paramref name="src"/>.</param>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <exception cref="ArgumentException">When the <paramref name="src"/> is empty.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void GetItems<T>(this Random random, in ReadOnlySpan<T> src, Span<T> dest)
    {
        if (src.IsEmpty)
            throw new ArgumentException("Source cannot be empty.", nameof(src));

        for (var i = 0; i < dest.Length; i++)
            dest[i] = src[random.Next(src.Length)];
    }

    /// <summary>
    /// Creates a random subset of the specified <paramref name="src"/> with the specified <paramref name="length"/>.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <param name="src">The values to use to populate the subset.</param>
    /// <param name="length">The length of the subset to return.</param>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <returns>A random subset populated with the specified <paramref name="src"/>.</returns>
    /// <exception cref="ArgumentException">When the <paramref name="src"/> is empty or <paramref name="length"/> is negative.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] GetItems<T>(this Random random, in ReadOnlySpan<T> src, int length)
    {
        if (src.IsEmpty)
            throw new ArgumentException("Source cannot be empty.", nameof(src));
        
        if (length < 0)
            throw new ArgumentException("Length cannot be negative.", nameof(length));

        var dest = new T[length];
        random.GetItems(src, dest);
        return dest;
    }

    /// <summary>
    /// Shuffles the specified <paramref name="values"/> using the Fisher-Yates algorithm.
    /// </summary>
    /// <param name="random">The current <see cref="Random"/> instance.</param>
    /// <param name="values">The values to shuffle.</param>
    /// <typeparam name="T">The type of the values.</typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Shuffle<T>(this Random random, Span<T> values)
    {
        var length = values.Length;
        
        for (var i = 0; i < length; i++)
        {
            var j = random.Next(i, length);

            if (j != i)
                (values[i], values[j]) = (values[j], values[i]);
        }
    }
}