using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components.Entities;

public struct Position : IVectorable<Position>
{
	public float X { get; set; }
	public float Y { get; set; }

    public static Position Zero => _zero;

    public static Position One => _one;

	private static readonly Position _zero = new();
	private static readonly Position _one = new(1.0f, 1.0f);

    public Position(float x, float y)
	{
		X = x;
		Y = y;
	}

	public static implicit operator Vector2(Position input)
		=> new(input.X, input.Y);

	public static implicit operator Position(Vector2 input)
		=> new(input.X, input.Y);
}