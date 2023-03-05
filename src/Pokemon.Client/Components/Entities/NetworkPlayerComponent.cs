
namespace Pokemon.Client.Components.Entities;

public readonly struct NetworkPlayerComponent
{
    public string Id { get; }
    public bool IsCurrentPlayer { get; }

    public NetworkPlayerComponent(string id, bool isCurrentPlayer)
    {
        Id = id;
        IsCurrentPlayer = isCurrentPlayer;
    }
}