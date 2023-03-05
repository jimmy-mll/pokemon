using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Authentication;

public sealed class IdentificationSuccessMessage : PokemonMessage
{
	public string Id { get; private set; }


	public new const ushort Identifier = 3;
	public override ushort MessageId =>
		Identifier;


	public IdentificationSuccessMessage() { }
	public IdentificationSuccessMessage(string id)
    {
        Id = id;
    }

    public override void Serialize(PokemonWriter writer)
    {
		writer.WriteString(Id);
    }

    public override void Deserialize(PokemonReader reader)
	{
		Id = reader.ReadString();
	}

}