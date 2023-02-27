using Raylib_CsLo;

namespace Pokemon.Engine.Input;

public static class Mouse
{
	public static MouseState GetState() =>
		new(Raylib.GetMousePosition(), Raylib.GetMouseWheelMoveV(), Raylib.GetGestureDetected_(), Raylib.GetMouseDelta());
}