using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;
using Pokemon.Protocol.Enums;

namespace Pokemon.Protocol.Messages.Authentication;

public sealed class IdentificationFailedMessage : PokemonMessage
{
	public new const ushort Identifier = 2;

	public override ushort MessageId =>
		Identifier;

	public IdentificationFailureReasons Reason { get; set; }

	public IdentificationFailedMessage()
	{
	}

	public IdentificationFailedMessage(IdentificationFailureReasons reason) =>
		Reason = reason;

	public override void Serialize(PokemonWriter writer)
	{
		writer.WriteEnum(Reason);
	}

	public override void Deserialize(PokemonReader reader)
	{
		Reason = reader.ReadEnum<IdentificationFailureReasons>();
	}
}