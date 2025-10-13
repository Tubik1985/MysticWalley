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
            var path = Path.Combine(FileSystem.AppDataDirectory, "whispers.json");

            // 🛠 Создаём файл только один раз — если его ещё нет
            if (!File.Exists(path))
            {
                await _whispers.InitializeAsync();
                Console.WriteLine($"[WhisperPage] whispers.json created at {path}");
            }
            else
            {
                Console.WriteLine($"[WhisperPage] Existing whispers file found: {path}");
            }

            // 📚 Загружаем все записи, накопленные в приложении
            var items = await _whispers.GetAllAsync();
            WhisperList.ItemsSource = items;

            Console.WriteLine($"[WhisperPage] Loaded {items.Count()} Whisper entries.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WhisperPage] Error loading whispers: {ex.Message}");
            await DisplayAlert("Ошибка", "Не удалось загрузить шёпоты долины.", "OK");
        }
    }
}