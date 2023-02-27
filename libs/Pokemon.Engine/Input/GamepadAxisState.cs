using System.Numerics;

namespace Pokemon.Engine.Input;

public readonly record struct GamepadAxisState(int AxisCount, Vector2 AxisLeft, Vector2 AxisRight)
{
    public GamepadAxisDirection LeftDirection =>
        AxisLeft switch
        {
            { X: > 0 } => GamepadAxisDirection.Right,
            { X: < 0 } => GamepadAxisDirection.Left,
            { Y: > 0 } => GamepadAxisDirection.Up,
            { Y: < 0 } => GamepadAxisDirection.Down,
            _ => GamepadAxisDirection.None
        };
    
    public GamepadAxisDirection RightDirection =>
        AxisRight switch
        {
            { X: > 0 } => GamepadAxisDirection.Right,
            { X: < 0 } => GamepadAxisDirection.Left,
            { Y: > 0 } => GamepadAxisDirection.Up,
            { Y: < 0 } => GamepadAxisDirection.Down,
            _ => GamepadAxisDirection.None
        };
}