using Pokemon.Client.Models.Network;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;
using Pokemon.Protocol.Extensions;

namespace Pokemon.Protocol.Messages.Spawning;

public sealed class NewPlayerSpawnedMessage : PokemonMessage
{
    public GamePlayer SpawnedPlayer { get; set; }

    public List<GamePlayer> OtherPlayers { get; set; }


    public new const ushort Identifier = 4;
    public override ushort MessageId => Identifier;

    public NewPlayerSpawnedMessage() { }
    public NewPlayerSpawnedMessage(GamePlayer spawnedPlayer, List<GamePlayer> otherPlayers)
    {
        SpawnedPlayer = spawnedPlayer;
        OtherPlayers = otherPlayers;
    }

    public override void Deserialize(PokemonReader reader)
    {
        SpawnedPlayer = new GamePlayer()
        {
            Id = reader.ReadString(),
            Position = reader.ReadVector2()
        };

        var count = reader.ReadUInt16();
        OtherPlayers = new List<GamePlayer>(count);
        for (int i = 0; i < count; i++)
        {
            OtherPlayers.Add(new GamePlayer()
            {
                Id = reader.ReadString(),
                Position = reader.ReadVector2()
            });
        }
    }

    public override void Serialize(PokemonWriter writer)
    {
        writer.WriteString(SpawnedPlayer.Id);
        writer.WriteVector2(SpawnedPlayer.Position);

        writer.WriteUInt16((ushort)OtherPlayers.Count);
        for (int i = 0; i < OtherPlayers.Count; i++)
        {
            var item = OtherPlayers[i];
            writer.WriteString(item.Id);
            writer.WriteVector2(item.Position);
        }
    }
}
