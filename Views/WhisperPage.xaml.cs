using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class WhisperPage : ContentPage
{
    private readonly WhisperService _whispers;

    // ❶ Пустой конструктор для Shell‑навигации
    public WhisperPage() : this(App.Services.GetService<WhisperService>()!) { }

    // ❷ Конструктор с внедрением зависимости
    public WhisperPage(WhisperService whispers)
    {
        InitializeComponent();
        _whispers = whispers;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // 🧹 Очистить старый файл при первой инициализации, чтобы не тащить старые данные
            // ⚠️ Оставь эту строку пока идёт отладка; потом можно закомментировать
         //   await _whispers.ClearAsync();

            // 💫 Создаёт локальную копию из ресурсов при первом запуске (если файла нет)
            await _whispers.InitializeAsync();

            // 📚 Загружаем все шёпоты и привязываем к списку
            var items = await _whispers.GetAllAsync();
            WhisperList.ItemsSource = items;

            Console.WriteLine($"[WhisperPage] Loaded {items.Count()} whispers.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WhisperPage] Error loading whispers: {ex.Message}");
            await DisplayAlert("Ошибка", "Не удалось загрузить шёпоты долины.", "OK");
        }
    }
}