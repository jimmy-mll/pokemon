using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Nemesis.Core.Network.Metadata;

namespace Nemesis.Core.Network.Framing;

/// <summary>
///     Describes a way to parse a network messages.
/// </summary>
public interface IMessageParser
{
	/// <summary>
	///     Decodes a message from the given <paramref name="sequence" />.
	/// </summary>
	/// <param name="sequence">The sequence buffer.</param>
	/// <param name="message">The decoded message.</param>
	/// <returns>Whether the message was decoded successfully.</returns>
	bool TryDecodeMessage(ReadOnlySequence<byte> sequence, [NotNullWhen(true)] out PokemonMessage? message);

	/// <summary>
	///     Encodes a network message into a <see cref="ReadOnlyMemory{T}" />.
	/// </summary>
	/// <param name="message">The message to encode.</param>
	/// <returns>A encoded message as an <see cref="ReadOnlyMemory{T}" />.</returns>
	ReadOnlyMemory<byte> TryEncodeMessage(PokemonMessage message);
}