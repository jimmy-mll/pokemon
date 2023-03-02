using Pokemon.Monogame.Models.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Pokemon.Monogame.Models;

public readonly struct AnimationGroupItem
{
    public string Key { get; }
    public int[] FrameIndices { get; }

    public AnimationGroupItem(string key, params int[] frameIndices)
    {
        Key = key; 
        FrameIndices = frameIndices;
    }
}

public readonly struct AnimationGroup
{
    public int FPS { get; }
    public string Name { get; }
    public bool IsLooping { get; }
    public SpriteSheet SpriteSheet { get; }

    private readonly Dictionary<string, Animation> _animations;

    public Animation this[string key]
        => _animations[key];

    public AnimationGroup(SpriteSheet spriteSheet, string name, bool isLooping, int fps, AnimationGroupItem[] animations)
    {
        FPS = fps;
        Name = name;
        IsLooping = isLooping;
        SpriteSheet = spriteSheet;

        _animations = new Dictionary<string, Animation>();
        for (int i = 0; i < animations.Length; i++)
        {
            var anim = new Animation(FPS, IsLooping, SpriteSheet, animations[i].FrameIndices);
            _animations.Add(animations[i].Key, anim);
        }
    }

    public static AnimationGroup FromFile(string path)
    {
        var stream = File.OpenRead(path);
        var data = JsonSerializer.Deserialize<AnimationGroupData>(stream);

        return data.GetValue();
    }

    public static async Task<AnimationGroup> FromFileAsync(string path)
    {
        var stream = File.OpenRead(path);
        var animationGroup = await JsonSerializer.DeserializeAsync<AnimationGroup>(stream);

        return animationGroup;
    }
}