namespace Pokemon.Monogame;

public readonly struct TextureRef
{
	private readonly int _id;

	private TextureRef(int id)
		=> _id = id;

	public static readonly TextureRef None = new(-1);

	public static implicit operator TextureRef(int id) => new(id);

	public static implicit operator int(TextureRef textureRef) => textureRef._id;
}