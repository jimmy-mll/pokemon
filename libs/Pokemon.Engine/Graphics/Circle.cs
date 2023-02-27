using System.Diagnostics;
using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Graphics;

[DebuggerDisplay("{ToString()}")]
public struct Circle
{
	public static readonly Circle Zero = new(Vector2.Zero, 0);
	
	public Vector2 Position { get; set; }
	
	public float Radius { get; set; }
	
	public Circle(Vector2 position, float radius)
	{
		Position = position;
		Radius = radius;
	}
	
	public void Deconstruct(out Vector2 position, out float radius)
	{
		position = Position;
		radius = Radius;
	}
	
	public bool Contains(Vector2 position) =>
		Raylib.CheckCollisionPointCircle(position, Position, Radius);
	
	public bool Contains(Rectangle rectangle) =>
		Raylib.CheckCollisionCircleRec(Position, Radius, rectangle._base);
	
	public bool Contains(Circle circle) =>
		Raylib.CheckCollisionCircles(Position, Radius, circle.Position, circle.Radius);
	
	public bool IsMouseOver() =>
		Raylib.CheckCollisionPointCircle(Raylib.GetMousePosition(), Position, Radius);
	
	public void Draw(Color color) =>
		Raylib.DrawCircleV(Position, Radius, color);
	
	public void DrawHollow(Color color) =>
		Raylib.DrawCircleLines((int)Position.X, (int)Position.Y,  Radius, color);
	
	public void DrawSides(Color color, int sides = 5, float rotation = 0) =>
		Raylib.DrawPoly(Position, sides, Radius, rotation, color);

	public override string ToString() =>
		$"X: {Position.X}, Y: {Position.Y}, Radius: {Radius}";
}