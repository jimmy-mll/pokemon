using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Spawning;

public sealed class NewPlayerSpawnedRequestMessage : PokemonMessage
{
    public string Id { get; set; }

    public new const ushort Identifier = 5;
    public override ushort MessageId => Identifier;

    public NewPlayerSpawnedRequestMessage() { }
    public NewPlayerSpawnedRequestMessage(string id)
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
