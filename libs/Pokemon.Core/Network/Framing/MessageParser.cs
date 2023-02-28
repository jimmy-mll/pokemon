using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Core.Network.Framing;

/// <inheritdoc />
public sealed class MessageParser : IMessageParser
{
	private const byte HeaderLength = sizeof(ushort) + sizeof(int);

	private readonly IMessageFactory _messageFactory;

	public MessageParser(IMessageFactory messageFactory) =>
		_messageFactory = messageFactory;

	/// <inheritdoc />
	public bool TryDecodeMessage(ReadOnlySequence<byte> sequence, [NotNullWhen(true)] out PokemonMessage? message)
	{
		message = null;

		var reader = new PokemonReader(sequence);

		if (reader.BytesAvailable < HeaderLength)
			return false;

		var messageId = reader.ReadUInt16();

		var messageLength = reader.ReadInt32();

		if (reader.BytesAvailable < messageLength)
			return false;

		if (!_messageFactory.TryGetMessage(messageId, out message))
			return false;

		message.Deserialize(reader);
		return true;
	}

	/// <inheritdoc />
	public ReadOnlyMemory<byte> TryEncodeMessage(PokemonMessage message)
	{
		var messageWriter = new PokemonWriter();
		message.Serialize(messageWriter);

		var payload = messageWriter.BufferAsSpan;

		var writer = new PokemonWriter();
		
		writer.WriteUInt16(message.MessageId);
		writer.WriteInt32(payload.Length);
		writer.WriteSpan(payload);

		return writer.BufferAsMemory;
	}
}