using System.Buffers;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Pokemon.Core.Serialization;

public sealed class PokemonReader
{
	private readonly ReadOnlyMemory<byte> _buffer;

	public int Length =>
		_buffer.Length;

	public int Position { get; private set; }

	public int BytesAvailable =>
		Length - Position;

	public PokemonReader(ReadOnlyMemory<byte> buffer) =>
		_buffer = buffer;

	public PokemonReader(ReadOnlySequence<byte> buffer) =>
		_buffer = SequenceMarshal.TryGetReadOnlyMemory(buffer, out var memory)
			? memory
			: buffer.ToArray();

	public PokemonReader(Stream stream)
	{
		if (stream is not MemoryStream ms)
		{
			ms = new MemoryStream();
			stream.CopyTo(ms);
		}

		_buffer = ms.ToArray();
	}

	public byte ReadUInt8() =>
		ReadSpan(sizeof(byte))[0];

	public sbyte ReadInt8() =>
		(sbyte)ReadSpan(sizeof(byte))[0];

	public bool ReadBoolean() =>
		ReadSpan(sizeof(byte))[0] is not 0;

	public ushort ReadUInt16() =>
		BinaryPrimitives.ReadUInt16LittleEndian(ReadSpan(sizeof(ushort)));

	public short ReadInt16() =>
		BinaryPrimitives.ReadInt16LittleEndian(ReadSpan(sizeof(short)));

	public uint ReadUInt32() =>
		BinaryPrimitives.ReadUInt32LittleEndian(ReadSpan(sizeof(uint)));

	public int ReadInt32() =>
		BinaryPrimitives.ReadInt32LittleEndian(ReadSpan(sizeof(int)));

	public ulong ReadUInt64() =>
		BinaryPrimitives.ReadUInt64LittleEndian(ReadSpan(sizeof(ulong)));

	public long ReadInt64() =>
		BinaryPrimitives.ReadInt64LittleEndian(ReadSpan(sizeof(long)));

	public float ReadFloat() =>
		BinaryPrimitives.ReadSingleLittleEndian(ReadSpan(sizeof(float)));

	public double ReadDouble() =>
		BinaryPrimitives.ReadDoubleLittleEndian(ReadSpan(sizeof(double)));

	public ReadOnlyMemory<byte> ReadMemory(int count)
	{
		var memory = _buffer.Slice(Position, count);
		Position += count;
		return memory;
	}

	public ReadOnlySpan<byte> ReadSpan(int count) =>
		ReadMemory(count).Span;

	public string ReadStringBytes(int count) =>
		Encoding.UTF8.GetString(ReadSpan(count));

	public string ReadString() =>
		ReadStringBytes(ReadUInt16());

	public string ReadBigString() =>
		ReadStringBytes(ReadInt32());
	
	public Vector2 ReadVector2() =>
		new(ReadFloat(), ReadFloat());
	
	public Vector3 ReadVector3() =>
		new(ReadFloat(), ReadFloat(), ReadFloat());
	
	public Vector4 ReadVector4() =>
		new(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());

	public void Seek(int offset, SeekOrigin origin) =>
		Position = origin switch
		{
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => Position + offset,
			SeekOrigin.End => _buffer.Length - Math.Abs(offset),
			_ => throw new ArgumentOutOfRangeException(nameof(origin))
		};
}