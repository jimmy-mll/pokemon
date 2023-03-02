using System.Linq;
using Pokemon.Monogame.Models.Json.Interfaces;

namespace Pokemon.Monogame.Models.Json;

public struct AnimationGroupData : IJsonData<AnimationGroup>
{
    public string Name { get; set; }
    public int FPS { get; set; }
    public bool IsLooping { get; set; }
    public SpriteSheetData SpriteSheet { get; set; }
    public AnimationGroupItemData[] Animations { get; set; }

    public AnimationGroup GetValue()
    {
        return new AnimationGroup(SpriteSheet.GetValue(), Name, IsLooping, FPS, Animations.Select(x => x.GetValue()).ToArray());
    }
}
