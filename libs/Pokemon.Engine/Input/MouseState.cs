using System.Numerics;
using Raylib_CsLo;

namespace Pokemon.Engine.Input;

public sealed record MouseState(Vector2 Position, Vector2 Wheel, Gesture Gesture, Vector2 Delta);