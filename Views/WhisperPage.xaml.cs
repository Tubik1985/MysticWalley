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

    // 🪤 Мини‑ловушка — максимально информативное логирование
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var path = Path.Combine(FileSystem.AppDataDirectory, "whispers.json");
            Console.WriteLine($"[WhisperPage] AppDataDirectory: {FileSystem.AppDataDirectory}");

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

            // 📚 Загружаем все записи
            var items = await _whispers.GetAllAsync();

            // Проверим на null или пустой результат — тоже может вызвать падение при биндинге
            if (items == null)
            {
                Console.WriteLine("[WhisperPage] Warning: whisper list is null.");
                await DisplayAlert("Диагностика", "Список шёпотов пуст (null).", "OK");
                return;
            }

            WhisperList.ItemsSource = items.ToList();
            Console.WriteLine($"[WhisperPage] Loaded {items.Count()} Whisper entries.");
        }
        catch (Exception ex)
        {
            // 🧾 Подробный отчёт
            var msg =
                $"Тип: {ex.GetType().Name}\n" +
                $"Сообщение: {ex.Message}\n\n" +
                $"StackTrace:\n{ex.StackTrace}";

            Console.WriteLine($"[WhisperPage] Error loading whispers: {msg}");
            await DisplayAlert("Ошибка при загрузке шёпотов", msg, "OK");
        }
    }
}