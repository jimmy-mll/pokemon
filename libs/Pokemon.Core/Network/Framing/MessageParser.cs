using System.Buffers;
using Pokemon.Core.Network.Metadata;

namespace Pokemon.Core.Network.Framing;

public class MessageParser : IMessageParser
{
	public bool TryDecodeMessage(ReadOnlySequence<byte> sequence, out PokemonMessage? message) =>
		throw new NotImplementedException();

	public ReadOnlyMemory<byte> TryEncodeMessage(PokemonMessage message) =>
		throw new NotImplementedException();
}