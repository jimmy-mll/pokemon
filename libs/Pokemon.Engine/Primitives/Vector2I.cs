using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Pokemon.Engine.Primitives;

public record struct Vector2I(int X, int Y) : IFormattable
{
	private const int Count = 2;

	public static Vector2I Zero =>
		new(0);

	public static Vector2I One =>
		new(1);

	public static Vector2I UnitX =>
		new(1, 0);

	public static Vector2I UnitY =>
		new(0, 1);

	public int this[int index]
	{
		get => GetElement(this, index);
		set => this = WithElement(this, index, value);
	}

	public Vector2I(int value) : this(value, value)
	{
	}

	public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format,
		IFormatProvider? formatProvider)
	{
		var separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;

		return $"<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}>";
	}

	private static int GetElement(Vector2I vector, int index)
	{
		if ((uint)index >= Count)
			throw new ArgumentOutOfRangeException(nameof(index), index,
				"Index was out of range. Must be non-negative and less than the size of the collection.");

		return GetElementUnsafe(ref vector, index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetElementUnsafe(ref Vector2I vector, int index)
	{
		Debug.Assert(index is >= 0 and < Count);
		return Unsafe.Add(ref Unsafe.As<Vector2I, int>(ref vector), index);
	}

	private static Vector2I WithElement(Vector2I vector, int index, int value)
	{
		if ((uint)index >= Count)
			throw new ArgumentOutOfRangeException(nameof(index), index,
				"Index was out of range. Must be non-negative and less than the size of the collection.");

		var result = vector;
		SetElementUnsafe(ref result, index, value);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void SetElementUnsafe(ref Vector2I vector, int index, int value)
	{
		Debug.Assert(index is >= 0 and < Count);
		Unsafe.Add(ref Unsafe.As<Vector2I, int>(ref vector), index) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator +(Vector2I left, Vector2I right) =>
		new(
			left.X + right.X,
			left.Y + right.Y
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator /(Vector2I left, Vector2I right) =>
		new(
			// ReSharper disable once PossibleLossOfFraction
			left.X / right.X,
			// ReSharper disable once PossibleLossOfFraction
			left.Y / right.Y
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator /(Vector2I value1, int value2) =>
		value1 / new Vector2I(value2);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator *(Vector2I left, Vector2I right) =>
		new(
			left.X * right.X,
			left.Y * right.Y
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator *(Vector2I left, int right) =>
		left * new Vector2I(right);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator *(int left, Vector2I right) =>
		right * left;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator -(Vector2I left, Vector2I right) =>
		new(
			left.X - right.X,
			left.Y - right.Y
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2I operator -(Vector2I value) =>
		Zero - value;

	public readonly override string ToString() =>
		ToString("G", CultureInfo.CurrentCulture);

	public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format) =>
		ToString(format, CultureInfo.CurrentCulture);
}