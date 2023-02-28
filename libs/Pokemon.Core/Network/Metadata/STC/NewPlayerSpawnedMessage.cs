namespace Pokemon.Core.Network.Metadata.STC;

public class NewPlayerSpawnedMessage : PokemonMessage
{
    public int NewPlayerId { get; }
    public int PositionX { get; }
    public int PositionY { get; }

    public override ushort MessageId => 5;

    public NewPlayerSpawnedMessage() { }
    public NewPlayerSpawnedMessage(int newPlayerId, int positionX, int positionY)
    {
        NewPlayerId = newPlayerId;
        PositionX = positionX;
        PositionY = positionY;
    }
}
