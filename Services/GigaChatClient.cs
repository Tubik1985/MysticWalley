using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MysticWalley.Services;

public class GigaChatClient
{
    private readonly HttpClient _http;
    private readonly GigaTokenService _tokenService;

    public GigaChatClient(GigaTokenService tokenService)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        _http = new HttpClient(handler);
        _tokenService = tokenService;
    }

    public async Task<string> GetPredictionAsync(string characterName, string userPrompt)
    {
        var token = await _tokenService.GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return "⚠ Не удалось получить access_token";

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var payload = new
        {
            model = "GigaChat:latest",
            messages = new object[]
            {
                new {
                    role = "system",
                    content = $"Ты — мистический персонаж {characterName}. " +
                              "Отвечай образно, таинственно, и коротко (1–3 предложения)."
                },
                new { role = "user", content = userPrompt }
            },
            max_tokens = 150
        };

        var json = JsonSerializer.Serialize(payload);

        using var resp = await _http.PostAsync(
            "https://gigachat.devices.sberbank.ru/api/v1/chat/completions",
            new StringContent(json, Encoding.UTF8, "application/json"));

        var rawResult = await resp.Content.ReadAsStringAsync();

        Console.WriteLine("==== RAW GIGACHAT RESPONSE ====");
        Console.WriteLine(rawResult);

        if (!resp.IsSuccessStatusCode)
            return $"⚠ Ошибка GigaChat: {resp.StatusCode}\n{rawResult}";

        try
        {
            using var doc = JsonDocument.Parse(rawResult);

            if (doc.RootElement.TryGetProperty("choices", out var choices)
                && choices.GetArrayLength() > 0
                && choices[0].TryGetProperty("message", out var msg)
                && msg.TryGetProperty("content", out var content))
            {
                return content.GetString() ?? "[Пустой ответ]";
            }
        }
        catch (Exception jex)
        {
            return $"⚠ Ошибка парсинга JSON: {jex.Message}\n{rawResult}";
        }

        return "[Структура ответа от GigaChat неожиданна]";
    }
}