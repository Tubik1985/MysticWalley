using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MysticWalley.Services;

public class GigaTokenService
{
    private string _token = string.Empty;
    private DateTime _expiresAt = DateTime.MinValue;

    // ⚠️ Подставь реальные значения из ЛК Сбера
    private readonly string _clientId = "019958bb-3c43-7243-909c-0666f2bc1c45";
    private readonly string _clientSecret = "4c7a21a4-14b3-4eb9-ad82-bd8b724cd1c1";

    // Модель токена
    private class GigaTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        [JsonPropertyName("expires_in")] public int? ExpiresIn { get; set; } // старый формат
        [JsonPropertyName("expires_at")] public long? ExpiresAt { get; set; } // новый (мс)
        [JsonPropertyName("token_type")] public string TokenType { get; set; }
        [JsonPropertyName("error")] public string Error { get; set; }
        [JsonPropertyName("error_description")] public string ErrorDescription { get; set; }
    }

    public async Task<string> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_token) || DateTime.UtcNow >= _expiresAt)
        {
            var handler = new HttpClientHandler
            {
                // ⚠️ только для отладки
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            using var http = new HttpClient(handler);

            var body = "scope=GIGACHAT_API_PERS";

            // правильный base64(client:secret)
            var encoded = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}")
            );

            var req = new HttpRequestMessage(HttpMethod.Post,
                "https://ngw.devices.sberbank.ru:9443/api/v2/oauth")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", encoded);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.Add("RqUID", Guid.NewGuid().ToString());

            var resp = await http.SendAsync(req);
            var rawText = await resp.Content.ReadAsStringAsync();

            Console.WriteLine("==== RAW TOKEN RESPONSE ====");
            Console.WriteLine(rawText);

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"Ошибка {resp.StatusCode}: {rawText}");

            var tokenResp = JsonSerializer.Deserialize<GigaTokenResponse>(rawText);

            if (!string.IsNullOrEmpty(tokenResp?.Error))
                throw new Exception($"{tokenResp.Error}: {tokenResp.ErrorDescription}");

            if (string.IsNullOrEmpty(tokenResp?.AccessToken))
                throw new Exception($"Нет access_token в ответе: {rawText}");

            _token = tokenResp.AccessToken;

            // срок действия токена
            if (tokenResp.ExpiresIn.HasValue)
            {
                _expiresAt = DateTime.UtcNow.AddSeconds(tokenResp.ExpiresIn.Value - 60);
            }
            else if (tokenResp.ExpiresAt.HasValue)
            {
                var expires = DateTimeOffset
                    .FromUnixTimeMilliseconds(tokenResp.ExpiresAt.Value).UtcDateTime;
                _expiresAt = expires.AddSeconds(-60);
            }
            else
            {
                _expiresAt = DateTime.UtcNow.AddMinutes(5);
            }

            Console.WriteLine($"[GigaTokenService] Токен валиден до {_expiresAt}");
        }

        return _token;
    }
}
