using System.Diagnostics;
using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Graphics;

[DebuggerDisplay("{ToString()}")]
public struct Rectangle
{
	public static Rectangle Zero =>
		new(Vector2.Zero, Vector2.Zero);

	public static Rectangle Max =>
		new(new Vector2(float.MinValue), new Vector2(float.MaxValue));
	
	private Vector2 _position;
	private Vector2 _size;
	internal Raylib_CsLo.Rectangle _base;

	public Vector2 Position
	{
		get => _position;
		set
		{
			_position = value;
			_base.x = value.X;
			_base.y = value.Y;
		}
	}

	public Vector2 Size
	{
		get => _size;
		set
		{
			_size = value;
			_base.width = value.X;
			_base.height = value.Y;
		}
	}

	public Vector2 Center =>
		Position + Size / 2;

	public Rectangle(Vector2 position, Vector2 size)
	{
		_base = new Raylib_CsLo.Rectangle(position.X, position.Y, size.X, size.Y);
		_position = position;
		_size = size;
	}

	public void Deconstruct(out Vector2 position, out Vector2 size)
	{
		position = Position;
		size = Size;
	}

	public bool Contains(Vector2 position) =>
		Raylib.CheckCollisionPointRec(position, _base);

	public bool Contains(Rectangle rectangle) =>
		Raylib.CheckCollisionRecs(_base, rectangle._base);

	public bool Contains(Circle circle) =>
		circle.Contains(this);
	
	public bool IsMouseOver() =>
		Raylib.CheckCollisionPointRec(Raylib.GetMousePosition(), _base);
	
	public Rectangle Shrink(float amount) =>
		new(new Vector2(Position.X + amount, Position.Y + amount), new Vector2(Size.X - amount * 2, Size.Y - amount * 2));
	
	public Rectangle Grow(float amount) =>
		new(new Vector2(Position.X - amount, Position.Y - amount), new Vector2(Size.X + amount * 2, Size.Y + amount * 2));
	
	public Rectangle Extend(Vector2 amount) =>
		new(new Vector2(Position.X - amount.X, Position.Y - amount.Y), new Vector2(Size.X + amount.X * 2, Size.Y + amount.Y * 2));
	
	public void Draw(Color color) =>
		Raylib.DrawRectangleRec(_base, color);
	
	public void Draw(Color color, float rotation) =>
		Raylib.DrawRectanglePro(_base, Vector2.Zero, rotation, color);
	
	public void Draw(Color color, Vector2 origin) =>
		Raylib.DrawRectanglePro(_base, origin, 0, color);
	
	public void Draw(Color color, Vector2 origin, float rotation) =>
		Raylib.DrawRectanglePro(_base, origin, rotation, color);
	
	public void DrawLines(Color color, float thickness) =>
		Raylib.DrawRectangleLinesEx(_base, thickness, color);
	
	public void DrawRounded(Color color, float roundness, int segments = 10) =>
		Raylib.DrawRectangleRounded(_base, roundness, segments, color);
	
	public void DrawRoundedLines(Color color, float roundness, float thickness, int segments = 10) =>
		Raylib.DrawRectangleRoundedLines(_base, roundness, segments, thickness, color);
	
	public void DrawGradient(Color one, Color two, Color three, Color four) =>
		Raylib.DrawRectangleGradientEx(_base, one, two, three, four);

	public void DrawGradient(Color one, Color two, bool isVertical = false)
	{
		if (isVertical)
			Raylib.DrawRectangleGradientV((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, one, two);
		else
			Raylib.DrawRectangleGradientH((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y, one, two);
	}

	public override string ToString() =>
		$"X: {Position.X}, Y: {Position.Y}, Width: {Size.X}, Height: {Size.Y}";
}