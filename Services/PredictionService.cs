namespace MysticWalley.Services;

public class PredictionService
{
    private readonly GigaChatClient _giga;

    public PredictionService(GigaChatClient gigaChatClient)
    {
        _giga = gigaChatClient;
    }

    public Task<string> GetPredictionAsync(string characterName)
    {
        return _giga.GetPredictionAsync(characterName, "Поделись мистическим предсказанием");
    }
}