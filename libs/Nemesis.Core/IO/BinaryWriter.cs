using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Nemesis.Core.IO;

/// <summary>
///     Writes custom data types as binary value in custom encoding.
/// </summary>
public sealed class BinaryWriter
{
	private byte[] _buffer;

	private int _position;

	/// <summary>
	///     Returns the buffer as a <see cref="ReadOnlyMemory{T}" />.
	/// </summary>
	public ReadOnlyMemory<byte> Buffer =>
		_buffer.AsMemory(0, _position);

	/// <summary>
	///     Initializes a new instance of the <see cref="BinaryWriter" /> class.
	/// </summary>
	public BinaryWriter() =>
		_buffer = Array.Empty<byte>();

	/// <summary>
	///     Initializes a new instance of the <see cref="BinaryWriter" /> class.
	/// </summary>
	/// <param name="capacity">The initial capacity of the buffer.</param>
	public BinaryWriter(int capacity) =>
		_buffer = new byte[capacity];

	/// <summary>
	///     Writes a structure of type <typeparamref name="T" /> into the buffer.
	/// </summary>
	/// <param name="value">The structure to write.</param>
	/// <param name="resize">When the buffer need to regrow.</param>
	/// <typeparam name="T">The type of the structure.</typeparam>
	/// <exception cref="ArgumentException">When <typeparamref name="T" /> is a reference type.</exception>
	/// <exception cref="IndexOutOfRangeException">When the buffer is too small to write the requested type.</exception>
	public void Write<T>(T value, bool resize = true) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			throw new ArgumentException("T must be a value type.", nameof(value));

		var sizeOfT = Unsafe.SizeOf<T>();

		if (resize)
			CheckAndResizeBuffer(sizeOfT);

		if (_buffer.Length - _position < sizeOfT)
			throw new IndexOutOfRangeException("Not enough space in buffer.");

		Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(_buffer.AsSpan(_position, sizeOfT)), value);

		_position += sizeOfT;
	}

	/// <summary>
	///     Writes an array of structures of type <typeparamref name="T" /> into the buffer.
	/// </summary>
	/// <param name="values">The structures to write.</param>
	/// <typeparam name="T">The type of the structure.</typeparam>
	/// <exception cref="ArgumentException">When <typeparamref name="T" /> is a reference type.</exception>
	/// <exception cref="IndexOutOfRangeException">When the buffer is too small to write the requested type.</exception>
	public void Write<T>(T[] values) where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			throw new ArgumentException("T must be a value type.", nameof(values));

		if (values.Length is 0)
		{
			Write(values.Length);
			return;
		}

		var sizeOfT = Unsafe.SizeOf<T>();
		var sizeOfArray = sizeOfT * values.Length;

		CheckAndResizeBuffer(sizeOfArray + sizeof(int));
		Write(values.Length, false);

		if (_buffer.Length - _position < sizeOfArray)
			throw new IndexOutOfRangeException("Not enough space in buffer.");

		Unsafe.CopyBlockUnaligned(
			ref MemoryMarshal.GetReference(_buffer.AsSpan(_position, sizeOfArray)),
			ref MemoryMarshal.GetReference(MemoryMarshal.AsBytes(values.AsSpan())),
			(uint)sizeOfArray);

		_position += sizeOfArray;
	}

	/// <summary>
	///     Writes a string into the buffer.
	/// </summary>
	/// <param name="value">The value to write.</param>
	/// <param name="encoding">The encoding to use when writes the string.</param>
	public void Write(string value, Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;

		var bytes = encoding.GetBytes(value);

		Write(bytes.Length);
		Write(bytes);
	}

	/// <summary>
	///     Checks if the buffer is big enough to write the requested amount of bytes.
	/// </summary>
	/// <remarks>
	///     If the buffer is too small, it will be resized.
	/// </remarks>
	/// <param name="count">The amount of bytes to write.</param>
	/// <exception cref="OutOfMemoryException">The requested operation would exceed the maximum array length.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckAndResizeBuffer(int count)
	{
		var bytesAvailable = _buffer.Length - _position;

		if (count <= bytesAvailable)
			return;

		var currentCount = _buffer.Length;
		var growBy = Math.Max(count, currentCount);

		if (count is 0)
			growBy = Math.Max(growBy, 256);

		var newCount = currentCount + growBy;

		if ((uint)newCount > int.MaxValue)
		{
			var needed = (uint)(currentCount - bytesAvailable + count);

			if (needed > Array.MaxLength)
				throw new OutOfMemoryException("The requested operation would exceed the maximum array length.");

			newCount = Array.MaxLength;
		}

		Array.Resize(ref _buffer, newCount);
	}
}