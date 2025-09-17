namespace MysticWalley.Services;

public class PredictionService
{
    private static readonly string[] _phrases =
    {
        "Сегодня хороший день для новых начинаний.",
        "Слушай свою интуицию — она тебя не подведёт.",
        "Мелочи сегодня важнее, чем кажутся.",
        "Избегай пустых разговоров — они крадут энергию."
    };

    private readonly Random _rnd = new();

    public Task<string> GetPredictionAsync(string characterName)
    {
        var phrase = _phrases[_rnd.Next(_phrases.Length)];
        return Task.FromResult($"{characterName} предсказывает: \"{phrase}\"");
    }
}