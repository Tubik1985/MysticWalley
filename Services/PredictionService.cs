namespace MysticWalley.Services;

public class PredictionService
{
    private readonly GigaChatClient _giga;

    public PredictionService(GigaChatClient gigaChatClient)
    {
        _giga = gigaChatClient;
    }

    /// <summary>
    /// Запрашивает у GigaChat личное, одностороннее предсказание.
    /// </summary>
    /// <param name="characterPrompt">Промпт, описывающий персонажа и ситуацию.</param>
    /// <returns>Строка с предсказанием.</returns>
    public Task<string?> GetPredictionAsync(string characterPrompt)
    {
        // Системный промпт настраивает AI на роль "оракула".
        const string systemPrompt = "Ты — мистический оракул. Поделись коротким, загадочным и глубоким предсказанием, основанным на полученной информации.";
        return _giga.GetPredictionAsync(characterPrompt, systemPrompt);
    }

    // =========================================================================
    // ЧТО ИЗМЕНИЛОСЬ: Я добавил этот новый метод.
    // =========================================================================
    /// <summary>
    /// Запрашивает у GigaChat сгенерированную "живую" реплику для "сериала".
    /// </summary>
    /// <param name="improvPrompt">Полный промпт-инструкция для "актера".</param>
    /// <returns>Сгенерированная уникальная реплика.</returns>
    public Task<string?> GetImprovisedLineAsync(string improvPrompt)
    {
        // Системный промпт здесь другой. Он настраивает AI на роль "актера".
        const string systemPrompt = "Ты — талантливый актер, играющий роль в мистической драме. Точно следуй инструкциям в промпте, чтобы сымпровизировать свою реплику.";
        return _giga.GetPredictionAsync(improvPrompt, systemPrompt);
    }
}