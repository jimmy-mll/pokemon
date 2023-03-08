using BinaryReader = Nemesis.Core.IO.BinaryReader;
using BinaryWriter = Nemesis.Core.IO.BinaryWriter;

namespace Nemesis.Core.Network.Metadata;

public abstract class PokemonMessage
{
	public const ushort Identifier = 0;

	public abstract ushort MessageId { get; }

	public virtual void Serialize(BinaryWriter writer)
	{
	}

	public virtual void Deserialize(BinaryReader reader)
	{
	}
}