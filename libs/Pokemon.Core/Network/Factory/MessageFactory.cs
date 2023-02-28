using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Pokemon.Core.Network.Metadata;

namespace Pokemon.Core.Network.Factory;

/// <inheritdoc />
public sealed class MessageFactory : IMessageFactory
{
	private static readonly Type MessageType = typeof(PokemonMessage);
	
	private readonly ConcurrentDictionary<ushort, Func<PokemonMessage>> _messages;
	private readonly ConcurrentDictionary<ushort, string> _messageNames;
	
	public MessageFactory()
	{
		_messages = new ConcurrentDictionary<ushort, Func<PokemonMessage>>();
		_messageNames = new ConcurrentDictionary<ushort, string>();
	}

	/// <inheritdoc />
	public void Initialize(Assembly assembly)
	{
		foreach (var type in assembly.GetTypes().Where(x => x.IsSubclassOf(MessageType)))
		{
			var messageId = Convert.ToUInt16(type.GetField("Identifier")?.GetValue(null));

			if (_messages.ContainsKey(messageId))
				throw new Exception("A message with the same id already exists.");
			
			var factory = Expression.Lambda<Func<PokemonMessage>>(Expression.New(type)).Compile();
			
			_messages.TryAdd(messageId, factory);
			_messageNames.TryAdd(messageId, type.Name);
		}
	}

	/// <inheritdoc />
	public bool TryGetMessage(ushort messageId, [NotNullWhen(true)] out PokemonMessage? message)
	{
		message = null;
		
		if (!_messages.TryGetValue(messageId, out var factory))
			return false;
		
		message = factory();
		return true;
	}
}