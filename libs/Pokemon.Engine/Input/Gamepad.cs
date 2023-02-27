using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Input;

public static class Gamepad
{
	public static GamepadAxisState GetAxisState(int gamepad) =>
		new(
			Raylib.GetGamepadAxisCount(gamepad),
			new Vector2(
				Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_X),
				Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_LEFT_Y)),
			new Vector2(
				Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_RIGHT_X),
				Raylib.GetGamepadAxisMovement(gamepad, GamepadAxis.GAMEPAD_AXIS_RIGHT_Y))
		);

	public static bool IsConnected(int gamepad) =>
		Raylib.IsGamepadAvailable(gamepad);

	public static bool IsButtonDown(int gamepad, GamepadButton button) =>
		Raylib.IsGamepadButtonDown(gamepad, (int)button);

	public static bool IsButtonPressed(int gamepad, GamepadButton button) =>
		Raylib.IsGamepadButtonPressed(gamepad, (int)button);

	public static bool IsButtonReleased(int gamepad, GamepadButton button) =>
		Raylib.IsGamepadButtonReleased(gamepad, (int)button);

	public static bool IsButtonUp(int gamepad, GamepadButton button) =>
		Raylib.IsGamepadButtonUp(gamepad, (int)button);

	public static unsafe string GetGamepadName(int gamepad) =>
		new(Raylib.GetGamepadName(gamepad));

	public static bool IsButtonComboDown(int gamepad, params GamepadButton[] buttons) =>
		buttons.All(button => IsButtonDown(gamepad, button));

	public static bool IsButtonComboPressed(int gamepad, params GamepadButton[] buttons) =>
		buttons.All(button => IsButtonPressed(gamepad, button));

	public static bool IsButtonComboReleased(int gamepad, params GamepadButton[] buttons) =>
		buttons.All(button => IsButtonReleased(gamepad, button));

	public static bool IsButtonComboUp(int gamepad, params GamepadButton[] buttons) =>
		buttons.All(button => IsButtonUp(gamepad, button));
}