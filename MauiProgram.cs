using Microsoft.Extensions.Logging;
using MysticWalley.Services;
using MysticWalley.Views;

namespace MysticWalley;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // регистрация сервисов
        builder.Services.AddSingleton<GigaTokenService>();
        builder.Services.AddSingleton<GigaChatClient>();
        builder.Services.AddSingleton<PredictionService>();

        builder.Services.AddSingleton<CharacterService>();
        builder.Services.AddSingleton<HistoryService>();

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<PredictionPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<TestTokenPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        // строим приложение
        var app = builder.Build();

        // ⚡ Проверка получения токена при запуске
        Task.Run(async () =>
        {
            try
            {
                var tokenService = app.Services.GetService<GigaTokenService>();
                if (tokenService != null)
                {
                    var token = await tokenService.GetTokenAsync();
                    Console.WriteLine($"[MauiProgram] УСПЕХ. Токен начинается с: {token.Substring(0, 20)}...");
                }
                else
                {
                    Console.WriteLine("[MauiProgram] Ошибка: GigaTokenService == null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiProgram] Ошибка получения токена: {ex.Message}");
            }
        });

        return app;
    }
}