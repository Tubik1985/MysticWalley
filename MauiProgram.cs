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
        builder.Services.AddSingleton<CharacterService>();
        builder.Services.AddSingleton<PredictionService>();
        builder.Services.AddSingleton<HistoryService>();

        // регистрация страниц
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<PredictionPage>();
        builder.Services.AddTransient<HistoryPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}