using Pokemon.Core.Network.Metadata;

namespace Pokemon.Protocol.Messages.Authentication;

public sealed class IdentificationSuccessMessage : PokemonMessage
{
	public new const ushort Identifier = 3;

	public override ushort MessageId =>
		Identifier;
}