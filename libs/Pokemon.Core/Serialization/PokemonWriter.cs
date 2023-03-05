using System.Buffers.Binary;
using System.Text;

namespace Pokemon.Core.Serialization;

public sealed class PokemonWriter
{
	private byte[] _buffer;

	public int Length =>
		_buffer.Length;

	public int Position { get; private set; }

	public int BytesAvailable =>
		Length - Position;

	public Memory<byte> BufferAsMemory =>
		_buffer.AsMemory(0, Position);

	public Span<byte> BufferAsSpan =>
		_buffer.AsSpan(0, Position);

	public byte[] BufferAsArray =>
		_buffer.AsSpan(0, Position).ToArray();

	public PokemonWriter() =>
		_buffer = Array.Empty<byte>();

	public void WriteUInt8(byte value) =>
		GetSpan(sizeof(byte))[0] = value;

	public void WriteInt8(sbyte value) =>
		GetSpan(sizeof(sbyte))[0] = (byte)value;

	public void WriteBoolean(bool value) =>
		GetSpan(sizeof(bool))[0] = (byte)(value ? 1 : 0);

	public void WriteUInt16(ushort value) =>
		BinaryPrimitives.WriteUInt16LittleEndian(GetSpan(sizeof(ushort)), value);

	public void WriteInt16(short value) =>
		BinaryPrimitives.WriteInt16LittleEndian(GetSpan(sizeof(short)), value);

	public void WriteUInt32(uint value) =>
		BinaryPrimitives.WriteUInt32LittleEndian(GetSpan(sizeof(uint)), value);

	public void WriteInt32(int value) =>
		BinaryPrimitives.WriteInt32LittleEndian(GetSpan(sizeof(int)), value);

	public void WriteUInt64(ulong value) =>
		BinaryPrimitives.WriteUInt64LittleEndian(GetSpan(sizeof(ulong)), value);

	public void WriteInt64(long value) =>
		BinaryPrimitives.WriteInt64LittleEndian(GetSpan(sizeof(long)), value);

	public void WriteFloat(float value) =>
		BinaryPrimitives.WriteSingleLittleEndian(GetSpan(sizeof(float)), value);

	public void WriteDouble(double value) =>
		BinaryPrimitives.WriteDoubleLittleEndian(GetSpan(sizeof(double)), value);

	public void WriteMemory(Memory<byte> value) =>
		WriteSpan(value.Span);

	public void WriteSpan(Span<byte> value) =>
		value.CopyTo(GetSpan(value.Length));

	public void WriteStringBytes(string value) =>
		WriteSpan(Encoding.UTF8.GetBytes(value));

	public void WriteString(string value)
	{
		var bytes = Encoding.UTF8.GetBytes(value);

		WriteUInt16((ushort)bytes.Length);
		WriteSpan(bytes);
	}

	public void WriteBigString(string value)
	{
		var bytes = Encoding.UTF8.GetBytes(value);

		WriteInt32(bytes.Length);
		WriteSpan(bytes);
	}

	public void WriteEnum<TEnum>(TEnum value)
		where TEnum : struct, Enum =>
		WriteInt32((int)(object)value);

	private Span<byte> GetSpan(int count)
	{
		CheckAndResizeBuffer(count);

		var span = _buffer.AsSpan(Position, count);

		Position += count;

		return span;
	}

	public void Seek(int offset, SeekOrigin origin)
	{
		switch (origin)
		{
			case SeekOrigin.Begin:
				CheckAndResizeBuffer(offset, offset);
				Position = offset;
				break;
			case SeekOrigin.Current:
				CheckAndResizeBuffer(offset);
				Position += offset;
				break;
			case SeekOrigin.End:
				Position = Length - Math.Abs(offset);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(origin), origin, "Invalid seek origin.");
		}
	}

	private void CheckAndResizeBuffer(int count, int? position = null)
	{
		position ??= Position;

		var bytesAvailable = Length - position.Value;

		if (count <= bytesAvailable)
			return;

		var currentCount = Length;
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