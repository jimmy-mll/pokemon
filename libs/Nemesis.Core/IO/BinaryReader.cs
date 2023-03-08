using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Nemesis.Core.IO;

/// <summary>
///     Reads custom data types as binary values in custom encoding.
/// </summary>
public sealed class BinaryReader
{
	private readonly ReadOnlyMemory<byte> _buffer;

	public int Length =>
		_buffer.Length;

	public int Position { get; private set; }

	public int Remaining =>
		_buffer.Length - Position;

	/// <summary>
	///     Initializes a new instance of the <see cref="BinaryReader" /> class.
	/// </summary>
	/// <param name="buffer">The buffer to read into.</param>
	public BinaryReader(ReadOnlyMemory<byte> buffer) =>
		_buffer = buffer;

	/// <summary>
	///     Initializes a new instance of the <see cref="BinaryReader" /> class.
	/// </summary>
	/// <param name="sequence">The sequence to convert in <see cref="ReadOnlyMemory{T}" />.</param>
	public BinaryReader(ReadOnlySequence<byte> sequence) =>
		_buffer = SequenceMarshal.TryGetReadOnlyMemory(sequence, out var memory) ? memory : sequence.ToArray();

	/// <summary>
	///     Reads a structure of type <typeparamref name="T" /> from the buffer.
	/// </summary>
	/// <typeparam name="T">The type of the structure.</typeparam>
	/// <returns>The structure read from the buffer.</returns>
	/// <exception cref="ArgumentException">When <typeparamref name="T" /> is a reference type.</exception>
	/// <exception cref="IndexOutOfRangeException">When the buffer is too small to read the requested type.</exception>
	public T Read<T>() where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			throw new ArgumentException("T must be a value type.", nameof(T));

		var size = Unsafe.SizeOf<T>();

		if (Remaining < size)
			throw new IndexOutOfRangeException("The buffer is too small to read the requested type.");

		var result = Unsafe.ReadUnaligned<T>(ref MemoryMarshal.GetReference(_buffer.Span.Slice(Position, size)));

		Position += size;

		return result;
	}

	/// <summary>
	///     Reads an array of structures of type <typeparamref name="T" /> from the buffer.
	/// </summary>
	/// <typeparam name="T">The type of the structures.</typeparam>
	/// <returns>The structures read from the buffer.</returns>
	/// <exception cref="ArgumentException">When <typeparamref name="T" /> is a reference type.</exception>
	/// <exception cref="IndexOutOfRangeException">When the buffer is too small to read the requested type.</exception>
	public T[] ReadArray<T>() where T : struct
	{
		if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
			throw new ArgumentException("T must be a value type.", nameof(T));

		var sizeOfResult = Read<int>();

		if (sizeOfResult <= 0)
			return Array.Empty<T>();

		var sizeOfT = Unsafe.SizeOf<T>();
		var sizeOfArray = sizeOfT * sizeOfResult;

		if (Remaining < sizeOfArray)
			throw new IndexOutOfRangeException("The buffer is too small to read the requested type.");

		var result = new T[sizeOfResult];

		Unsafe.CopyBlockUnaligned(
			ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(result.AsSpan())),
			ref MemoryMarshal.GetReference(_buffer.Span.Slice(Position, sizeOfArray)),
			(uint)sizeOfArray);

		Position += sizeOfArray;

		return result;
	}

	/// <summary>
	///     Reads a string from the buffer.
	/// </summary>
	/// <param name="encoding">The encoding to use when reading the string.</param>
	/// <returns>The string read from the buffer.</returns>
	/// <exception cref="IndexOutOfRangeException">The buffer is too small to read the requested type.</exception>
	public string ReadString(Encoding? encoding = null)
	{
		encoding ??= Encoding.UTF8;

		var length = Read<int>();

		if (Remaining < length)
			throw new IndexOutOfRangeException("The buffer is too small to read the requested type.");

		var result = encoding.GetString(_buffer.Span.Slice(Position, length));

		Position += length;

		return result;
	}
}