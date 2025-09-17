namespace MysticWalley;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // регистрация маршрута для страницы предсказаний
        Routing.RegisterRoute(nameof(Views.PredictionPage), typeof(Views.PredictionPage));
    }
}