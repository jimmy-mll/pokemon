using Microsoft.Xna.Framework;

namespace Pokemon.Monogame.ECS.Components;

public struct Position : IVectorable<Position>
{
	public float X { get; set; }
	public float Y { get; set; }

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