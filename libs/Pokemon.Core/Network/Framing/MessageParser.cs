using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Pokemon.Core.Network.Factory;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Core.Network.Framing;

/// <inheritdoc />
public sealed class MessageParser : IMessageParser
{
	private const int HeaderSize = sizeof(ushort) + sizeof(int);

	private readonly IMessageFactory _messageFactory;

	/// <summary>Initializes a new instance of the <see cref="MessageParser" /> class.</summary>
	/// <param name="messageFactory">The message factory.</param>
	public MessageParser(IMessageFactory messageFactory) =>
		_messageFactory = messageFactory;

	/// <inheritdoc />
	public bool TryDecodeMessage(ReadOnlySequence<byte> sequence, [NotNullWhen(true)] out PokemonMessage? message)
	{
		message = null;

		if (sequence.Length < HeaderSize)
			return false;

		var reader = new PokemonReader(sequence);

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
		var contentWriter = new PokemonWriter();
		message.Serialize(contentWriter);

		var content = contentWriter.BufferAsSpan;

		var writer = new PokemonWriter();

		writer.WriteUInt16(message.MessageId);
		writer.WriteInt32(content.Length);
		writer.WriteSpan(content);

		return writer.BufferAsMemory;
	}
}