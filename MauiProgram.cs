using Microsoft.Extensions.Logging;
using MysticWalley.Services;
using MysticWalley.ViewModels;
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
        builder.Logging.AddDebug();
#endif

        // ===== РЕГИСТРАЦИЯ ЗАВИСИМОСТЕЙ (Dependency Injection) ==================

        // --- Сервисы (Singleton) ---
        builder.Services.AddSingleton<GameStateService>(); // <-- ИЗМЕНЕНИЕ №1: Зарегистрирован новый сервис
        builder.Services.AddSingleton<GigaTokenService>();
        builder.Services.AddSingleton<GigaChatClient>();
        builder.Services.AddSingleton<PredictionService>();
        builder.Services.AddSingleton<CharacterService>();
        builder.Services.AddSingleton<HistoryService>();
        builder.Services.AddSingleton<WhisperService>();
        builder.Services.AddSingleton<StoryService>();

        // --- ViewModels (Transient) ---
        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<WhisperViewModel>();
        // builder.Services.AddTransient<MapViewModel>(); // Пока не используем, но здесь ему место

        // --- Страницы (Views) ---
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<PredictionPage>();
        builder.Services.AddTransient<HistoryPage>();
        builder.Services.AddTransient<WhisperPage>();
        builder.Services.AddTransient<TestTokenPage>();
        // builder.Services.AddTransient<MapPage>(); // Пока не используем

        // ========================================================================

        var app = builder.Build();

        // Запускаем фоновую задачу для асинхронной инициализации
        InitializeServicesAsync(app.Services);

        return app;
    }

    private static void InitializeServicesAsync(IServiceProvider services)
    {
        Task.Run(async () =>
        {
            // --- ИЗМЕНЕНИЕ №2: Добавлен блок для загрузки состояния игры ---
            try
            {
                Console.WriteLine("[MauiProgram] Попытка загрузки состояния игры...");
                var gameStateService = services.GetRequiredService<GameStateService>();
                await gameStateService.LoadStateAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MauiProgram] КРИТИЧЕСКАЯ ОШИБКА при загрузке состояния игры: {ex.Message}");
            }
            // -----------------------------------------------------------

            try
            {
                Console.WriteLine("[MauiProgram] Попытка инициализации Giga-токена...");
                var tokenService = services.GetRequiredService<GigaTokenService>();
                var token = await tokenService.GetTokenAsync();

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