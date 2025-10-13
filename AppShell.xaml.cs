using MysticWalley.Views;   // добавляем в самом верху файла

namespace MysticWalley;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // маршруты страниц
        Routing.RegisterRoute(nameof(PredictionPage), typeof(PredictionPage));
        Routing.RegisterRoute(nameof(WhisperPage), typeof(WhisperPage));
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));  // если есть страница истории
    }
}