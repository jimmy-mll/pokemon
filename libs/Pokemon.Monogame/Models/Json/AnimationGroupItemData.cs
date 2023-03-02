using Pokemon.Monogame.Models.Json.Interfaces;

namespace Pokemon.Monogame.Models.Json;

public struct AnimationGroupItemData : IJsonData<AnimationGroupItem>
{
    public string Key { get; set; }
    public int[] FrameIndices { get; set; }

    public AnimationGroupItem GetValue()
    {
        return new AnimationGroupItem(Key, FrameIndices);
    }
}
