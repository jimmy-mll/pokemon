using Pokemon.Core.Serialization;

namespace Pokemon.Core.Network.Metadata;

public abstract class PokemonMessage
{
	public const ushort Identifier = 0;

	public abstract ushort MessageId { get; }

	public virtual void Serialize(PokemonWriter writer)
	{
	}

	public virtual void Deserialize(PokemonReader reader)
	{
	}
}