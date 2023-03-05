using Microsoft.Xna.Framework;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Extensions;

public static class PokemonWriterExtensions
{
    public static void WriteVector2(this PokemonWriter writer, Vector2 value)
    {
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
    }

    public static void WriteVector3(this PokemonWriter writer, Vector3 value)
    {
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
        writer.WriteFloat(value.Z);
    }

    public static void WriteVector4(this PokemonWriter writer, Vector4 value)
    {
        writer.WriteFloat(value.X);
        writer.WriteFloat(value.Y);
        writer.WriteFloat(value.Z);
        writer.WriteFloat(value.W);
    }
}
