using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Player;

public sealed class PlayerSpawnRequestMessage : PokemonMessage
{
	public new const ushort Identifier = 1;

	public override ushort MessageId =>
		Identifier;
	
	public int PlayerId { get; set; }
	
	public PlayerSpawnRequestMessage()
	{
	}
	
	public PlayerSpawnRequestMessage(int playerId) =>
		PlayerId = playerId;
	
	public override void Serialize(PokemonWriter writer)
	{
		writer.WriteInt32(PlayerId);
	}
	
	public override void Deserialize(PokemonReader reader)
	{
		PlayerId = reader.ReadInt32();
	}
}