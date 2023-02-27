using System.Numerics;

namespace Pokemon.Engine.Input;

public record struct GamepadAxisState(int AxisCount, Vector2 AxisLeft, Vector2 AxisRight);