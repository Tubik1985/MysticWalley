using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class TestTokenPage : ContentPage
{
    private readonly GigaTokenService _tokenService;

    public TestTokenPage(GigaTokenService tokenService)
    {
        InitializeComponent();
        _tokenService = tokenService;
    }

    private async void OnGetTokenClicked(object sender, EventArgs e)
    {
        try
        {
            var token = await _tokenService.GetTokenAsync();

            // Если получили строку access_token
            if (!string.IsNullOrEmpty(token) && !token.StartsWith("Ошибка") && !token.StartsWith("Exception") && !token.StartsWith("HttpRequestException"))
            {
                await DisplayAlert("УСПЕХ ✅", "Токен получен:\n" + token.Substring(0, 20) + "...", "OK");
            }
            else
            {
                // Если вместо токена вернулась ошибка (мы подставляем текст ошибки в catch)
                await DisplayAlert("Ошибка ❌", token ?? "Неизвестная ошибка", "OK");
            }
        }
        catch (HttpRequestException httpEx)
        {
            await DisplayAlert("HttpRequestException", httpEx.Message + "\n\nINNER:\n" + httpEx.InnerException?.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exception", ex.ToString(), "OK");
        }
    }
}