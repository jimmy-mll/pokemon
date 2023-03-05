using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Spawning;

public sealed class UnspawnedPlayerMessage : PokemonMessage
{
    public string UnspawnedPlayerId { get; set; }


    public new const ushort Identifier = 6;
    public override ushort MessageId => Identifier;

    public UnspawnedPlayerMessage() { }

    public UnspawnedPlayerMessage(string unspawnedPlayerId)
    {
        UnspawnedPlayerId = unspawnedPlayerId;
    }

    public override void Deserialize(PokemonReader reader)
    {
        UnspawnedPlayerId = reader.ReadString();
    }

    public override void Serialize(PokemonWriter writer)
    {
        writer.WriteString(UnspawnedPlayerId);
    }
}
