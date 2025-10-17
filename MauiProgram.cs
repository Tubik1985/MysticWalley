using Microsoft.Extensions.Logging;
using MysticWalley.Services;
using MysticWalley.ViewModels; // <-- Добавлен для единообразия
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

#if DEBUG
        // Включаем логирование отладки только в DEBUG-сборках
        builder.Logging.AddDebug();
#endif

        // ===== РЕГИСТРАЦИЯ ЗАВИСИМОСТЕЙ (Dependency Injection) ==================

        // --- Сервисы (Singleton): живут на протяжении всего жизненного цикла приложения.
        // Ядро приложения и работа с данными.
        builder.Services.AddSingleton<GigaTokenService>();
        builder.Services.AddSingleton<GigaChatClient>();
        builder.Services.AddSingleton<PredictionService>();
        builder.Services.AddSingleton<CharacterService>();
        builder.Services.AddSingleton<HistoryService>();
        builder.Services.AddSingleton<WhisperService>();
        builder.Services.AddSingleton<StoryService>();

        // --- ViewModels (Transient): создаются заново для каждой новой страницы.
        // Логика представления, состояние экрана.
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<WhisperViewModel>(); // <-- Наша новая регистрация

        // --- Страницы (Views) ---
        // Singleton для главной страницы, если она должна быть одна и та же.
        builder.Services.AddSingleton<MainPage>();

        // Transient для страниц, которые должны быть "свежими" при каждом заходе.
        builder.Services.AddTransient<PredictionPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<WhisperPage>(); // <-- Добавлено для полноты
        builder.Services.AddTransient<TestTokenPage>();
        // builder.Services.AddTransient<RitualPage>();

        // ========================================================================

        var app = builder.Build();

        // Запускаем фоновую задачу для асинхронной инициализации
        InitializeServicesAsync(app.Services);

        return app;
    }

    /// <summary>
    /// Асинхронно выполняет задачи, которые не должны блокировать запуск UI,
    /// например, получение токена.
    /// </summary>
    private static void InitializeServicesAsync(IServiceProvider services)
    {
        Task.Run(async () =>
        {
            try
            {
                // Используем IServiceProvider для получения сервиса, это чище.
                var tokenService = services.GetRequiredService<GigaTokenService>();
                var token = await tokenService.GetTokenAsync();

                // Выводим только часть токена для безопасности
                var tokenPreview = token.Length > 20 ? token.Substring(0, 20) : token;
                Console.WriteLine($"[MauiProgram] Успешная инициализация Giga-токена (начинается с: {tokenPreview}...).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiProgram] КРИТИЧЕСКАЯ ОШИБКА при инициализации Giga-токена: {ex.Message}");
            }
        });
    }
}