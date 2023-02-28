using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Pokemon.Core.Network.Metadata;

namespace Pokemon.Core.Network.Factory;

public interface IMessageFactory
{
	/// <summary>Registers all messages from the given <paramref name="assembly" />.</summary>
	/// <param name="assembly">The assembly to find messages in.</param>
	void Initialize(Assembly assembly);
	
	/// <summary>Attempts to get a <paramref name="message" /> based on its <paramref name="messageId" />.</summary>
	/// <param name="messageId">The id of the message.</param>
	/// <param name="message">The found message.</param>
	/// <returns><see langword="true" /> if the message was found; otherwise, <see langword="false" />.</returns>
	bool TryGetMessage(ushort messageId, [NotNullWhen(true)] out PokemonMessage? message);
}