using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components.Entities;

public struct Scale : IVectorable<Scale>
{
	public float X { get; set; }
	public float Y { get; set; }

    public static Scale Zero => _zero;

    public static Scale One => _one;

    private static readonly Scale _zero = new();
    private static readonly Scale _one = new(1.0f, 1.0f);

    public Scale() : this(1.0f, 1.0f)
	{
	}

	public Scale(float ratio) : this(ratio, ratio)
	{
	}

	public Scale(float x, float y)
	{
		X = x;
		Y = y;
	}

	public static implicit operator Vector2(Scale input)
		=> new(input.Y, input.Y);

	public static implicit operator Scale(Vector2 input)
		=> new(input.X, input.Y);
}