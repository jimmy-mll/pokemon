using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Nemesis.Core.Network.Factory;
using Nemesis.Core.Network.Metadata;
using BinaryReader = Nemesis.Core.IO.BinaryReader;
using BinaryWriter = Nemesis.Core.IO.BinaryWriter;

namespace Nemesis.Core.Network.Framing;

/// <inheritdoc />
public sealed class MessageParser : IMessageParser
{
	private const int HeaderSize = sizeof(ushort) + sizeof(int);

	private readonly IMessageFactory _messageFactory;

	/// <summary>
	///     Initializes a new instance of the <see cref="MessageParser" /> class.
	/// </summary>
	/// <param name="messageFactory">The message factory.</param>
	public MessageParser(IMessageFactory messageFactory) =>
		_messageFactory = messageFactory;

	/// <inheritdoc />
	public bool TryDecodeMessage(ReadOnlySequence<byte> sequence, [NotNullWhen(true)] out PokemonMessage? message)
	{
		message = null;

		if (sequence.Length < HeaderSize)
			return false;

		var reader = new BinaryReader(sequence);

		var messageId = reader.Read<ushort>();
		var messageLength = reader.Read<int>();

		if (reader.Remaining < messageLength)
			return false;

		if (!_messageFactory.TryGetMessage(messageId, out message))
			return false;

		message.Deserialize(reader);
		return true;
	}

	/// <inheritdoc />
	public ReadOnlyMemory<byte> TryEncodeMessage(PokemonMessage message)
	{
		var contentWriter = new BinaryWriter();
		message.Serialize(contentWriter);

		var content = contentWriter.Buffer;

		var writer = new BinaryWriter();

		writer.Write(message.MessageId);
		writer.Write(content.Length);
		writer.WriteArray(content.ToArray());

		return writer.Buffer;
	}
}