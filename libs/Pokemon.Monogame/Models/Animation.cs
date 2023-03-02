using System;
using System.Globalization;

namespace Pokemon.Monogame.Models;

public readonly struct Animation : IEquatable<Animation>
{
    public bool IsLooping { get; }
    public int FramesPerSecond { get; }
    public int[] FrameIndices { get; }
    public SpriteSheet Spritesheet { get; }

    public Animation(int framesPerSecond, bool isLooping, SpriteSheet spriteSheet, params int[] frameIndices)
    {
        IsLooping = isLooping;
        FramesPerSecond = framesPerSecond;
        Spritesheet = spriteSheet;
        FrameIndices = frameIndices;
    }

    public bool Equals(Animation other)
    {
        return IsLooping == other.IsLooping &&
               FramesPerSecond == other.FramesPerSecond &&
               FrameIndices == other.FrameIndices &&
               Spritesheet.Equals(other.Spritesheet);
    }

    public override bool Equals(object obj)
    {
        return obj is Animation && Equals((Animation)obj);
    }

    public static bool operator ==(Animation left, Animation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Animation left, Animation right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return IsLooping.GetHashCode() ^
               FramesPerSecond.GetHashCode() ^
               FrameIndices.GetHashCode();
    }
}
