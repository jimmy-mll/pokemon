using System.Numerics;
using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Player;

public sealed class PlayerSpawnMessage : PokemonMessage
{
	public new const ushort Identifier = 2;

	public override ushort MessageId =>
		Identifier;

	public Vector2 Position { get; set; }

	public PlayerSpawnMessage() { }
	
	public PlayerSpawnMessage(Vector2 position)
	{
		Position = position;
	}

	public override void Serialize(PokemonWriter writer)
	{
		writer.WriteFloat(Position.X);
		writer.WriteFloat(Position.Y);
	}
	
	public override void Deserialize(PokemonReader reader)
	{
		Position = new Vector2(reader.ReadFloat(), reader.ReadFloat());
	}
}