// Файл: Services/GigaTokenService.cs

using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MysticWalley.Services;

public class GigaTokenService
{
    private string _token = string.Empty;
    private DateTime _expiresAt = DateTime.MinValue;
    private readonly string _apiAuthKey;

    // ========================================================================
    // ВНУТРЕННИЙ КЛАСС ДЛЯ ПАРСИНГА ОТВЕТА. УБЕДИСЬ, ЧТО ОН ВЫГЛЯДИТ ИМЕННО ТАК.
    // ========================================================================
    private class GigaTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_at")]
        public long? ExpiresAt { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("error_description")]
        public string? ErrorDescription { get; set; }
    }

    public GigaTokenService(IConfiguration configuration)
    {
        _apiAuthKey = configuration["GigaChat:ApiKey"]
                      ?? throw new InvalidOperationException("GigaChat API Key not found. Please set it using 'dotnet user-secrets set \"GigaChat:ApiKey\" \"YOUR_KEY\"'");
    }

    public async Task<string> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_token) || DateTime.UtcNow >= _expiresAt)
        {
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            using var http = new HttpClient(handler);
            var body = "scope=GIGACHAT_API_PERS";

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(_apiAuthKey));

            var req = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", encoded);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            req.Headers.Add("RqUID", Guid.NewGuid().ToString());

            var resp = await http.SendAsync(req);
            var rawText = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
                throw new Exception($"GigaChat Auth Error {resp.StatusCode}: {rawText}");

            var tokenResp = JsonSerializer.Deserialize<GigaTokenResponse>(rawText);
            if (tokenResp == null)
                throw new Exception($"Failed to deserialize GigaChat response: {rawText}");

            if (!string.IsNullOrEmpty(tokenResp.Error))
                throw new Exception($"GigaChat Auth Error: {tokenResp.Error} - {tokenResp.ErrorDescription}");

            if (string.IsNullOrEmpty(tokenResp.AccessToken))
                throw new Exception($"AccessToken is missing in GigaChat response: {rawText}");

            _token = tokenResp.AccessToken;

            if (tokenResp.ExpiresAt.HasValue)
            {
                var expires = DateTimeOffset.FromUnixTimeMilliseconds(tokenResp.ExpiresAt.Value).UtcDateTime;
                _expiresAt = expires.AddSeconds(-60); // Берем запас в 60 секунд
            }
            else
            {
                _expiresAt = DateTime.UtcNow.AddMinutes(25); // Запасной вариант, если expires_at не пришел
            }
        }
        return _token;
    }
}