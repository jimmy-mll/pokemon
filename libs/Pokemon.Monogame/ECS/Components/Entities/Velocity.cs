using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components.Entities;

public struct Velocity : IVectorable<Velocity>
{
	public float X { get; set; }
	public float Y { get; set; }

    public static Velocity Zero => _zero;

    public static Velocity One => _one;

    private static readonly Velocity _zero = new();
    private static readonly Velocity _one = new(1.0f, 1.0f);

    public Velocity(float x, float y)
	{
		X = x;
		Y = y;
	}

	public static implicit operator Vector2(Velocity input)
		=> new(input.X, input.Y);

	public static implicit operator Velocity(Vector2 input)
		=> new(input.X, input.Y);
}