using Pokemon.Core.Network.Metadata;

namespace Pokemon.Protocol.Messages.Authentication;

public class HelloConnectMessage : PokemonMessage
{
	public override ushort MessageId { get; }
}