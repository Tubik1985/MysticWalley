namespace MysticWalley
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        // даём всему приложению доступ к DI‑контейнеру
        public static IServiceProvider Services
            => Current?.Handler?.MauiContext?.Services;
    }
}