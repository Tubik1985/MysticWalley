using MysticWalley.Services;

namespace MysticWalley.Views
{
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

        // 🔹 Загружаем данные при каждом появлении страницы
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

        // 🧹 Очистка архива шёпотов
        private async void OnClearWhispersClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Подтверждение",
                "Удалить все шёпоты долины?",
                "Да", "Отмена");

            if (!confirm)
                return;

            try
            {
                await _whispers.ClearAsync();

                // Перечитываем список — теперь он должен быть пуст
                var items = await _whispers.GetAllAsync();
                WhisperList.ItemsSource = items;

                await DisplayAlert("Готово", "Шёпоты долины очищены.", "OK");
                Console.WriteLine("[WhisperPage] All whispers cleared by user.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhisperPage] Clear action error: {ex.Message}");
                await DisplayAlert("Ошибка", "Не удалось очистить шёпоты долины.", "OK");
            }
        }
    }
}