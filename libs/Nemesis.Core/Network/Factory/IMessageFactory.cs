using System.Diagnostics.CodeAnalysis;
using Nemesis.Core.Network.Metadata;

namespace Nemesis.Core.Network.Factory;

/// <summary>
///     Represents a factory that can create <see cref="PokemonMessage" />.
/// </summary>
public interface IMessageFactory
{
	/// <summary>
	///     Attempts to get a <paramref name="message" /> based on its <paramref name="messageId" />.
	/// </summary>
	/// <param name="messageId">The id of the message.</param>
	/// <param name="message">The found message.</param>
	/// <returns><see langword="true" /> if the message was found; otherwise, <see langword="false" />.</returns>
	bool TryGetMessage(ushort messageId, [NotNullWhen(true)] out PokemonMessage? message);

	/// <summary>
	///     Attempts to get the name of a message based on its <paramref name="messageId" />.
	/// </summary>
	/// <param name="messageId">The id of the message.</param>
	/// <param name="messageName">The found message name.</param>
	/// <returns><see langword="true" /> if the message name was found; otherwise, <see langword="false" />.</returns>
	bool TryGetMessageName(ushort messageId, [NotNullWhen(true)] out string? messageName);
}