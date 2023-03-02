namespace Pokemon.Monogame.Models.Json.Interfaces;

public interface IJsonData<TValue>
{
    TValue GetValue();
}
