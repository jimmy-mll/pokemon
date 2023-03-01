namespace Pokemon.Monogame;

public readonly struct TextureRef
{
    public readonly int id;

    public TextureRef(int id)
        => this.id = id;

    public static readonly TextureRef None = new(-1);

    public static implicit operator TextureRef(int id) => new(id);

    public static implicit operator int(TextureRef textureRef) => textureRef.id;
}
