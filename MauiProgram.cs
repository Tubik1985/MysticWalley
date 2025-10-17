using Microsoft.Extensions.Logging;
using MysticWalley.Services;
using MysticWalley.Views;
using MysticWalley.ViewModels;
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

        // ===== Сервисы ===================================================
        builder.Services.AddSingleton<GigaTokenService>();
        builder.Services.AddSingleton<GigaChatClient>();
        builder.Services.AddSingleton<PredictionService>();
        builder.Services.AddSingleton<CharacterService>();
        builder.Services.AddSingleton<HistoryService>();
        builder.Services.AddSingleton<WhisperService>();
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddSingleton<StoryService>();
       
        // ===== Страницы ===================================================
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<PredictionPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<TestTokenPage>();
        // builder.Services.AddTransient<RitualPage>();  // если появится

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // Проверка Giga‑токена (асинхронный запуск)
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