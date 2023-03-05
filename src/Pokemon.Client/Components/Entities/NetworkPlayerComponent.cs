
namespace Pokemon.Client.Components.Entities;

public struct NetworkPlayerComponent
{
    public string Id { get; set; }

    public NetworkPlayerComponent(string id)
    {
        Id = id;
    }
}
