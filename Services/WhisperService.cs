using System.Text.Json;

namespace MysticWalley.Services
{
    public class WhisperService
    {
        private readonly string _filePath =
            Path.Combine(FileSystem.AppDataDirectory, "whispers.json");

        private readonly List<WhisperEntry> _entries = new();

        // 💫 Инициализация: создаёт копию ресурса при первом запуске
        public async Task InitializeAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    const string resourceFile = "Data/WhisperService.json";
                    using var stream = await FileSystem.OpenAppPackageFileAsync(resourceFile);
                    using var reader = new StreamReader(stream);
                    var json = await reader.ReadToEndAsync();
                    await File.WriteAllTextAsync(_filePath, json);
                    Console.WriteLine($"[WhisperService] Copied {resourceFile} → {_filePath}");
                }

                await LoadAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhisperService] Initialize error: {ex.Message}");
            }
        }

        // 📚 Получить все шёпоты
        public async Task<IEnumerable<WhisperEntry>> GetAllAsync()
        {
            await LoadAsync();
            return _entries.OrderByDescending(x => x.Time);
        }

        // 🪶 Добавить новую импровизацию
        public async Task AddImprovisationAsync(string hero, string emotion, string text)
        {
            try
            {
                await InitializeAsync();
                await LoadAsync();

                _entries.Add(new WhisperEntry
                {
                    Time = DateTime.Now,
                    Hero = hero,
                    Emotion = emotion,
                    Text = text
                });

                var json = JsonSerializer.Serialize(_entries,
                    new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(_filePath, json);
                Console.WriteLine($"[WhisperService] Added improvisation for {hero}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhisperService] AddImprovisation error: {ex.Message}");
            }
        }

        // 🧹 Очистить локальный архив шёпотов
        public async Task ClearAsync()
        {
            try
            {
                _entries.Clear();

                // Перезаписываем файл пустым массивом, чтобы он существовал всегда
                await File.WriteAllTextAsync(_filePath, "[]");
                Console.WriteLine($"[WhisperService] Cleared {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhisperService] ClearAsync error: {ex.Message}");
            }
        }

        // 🔧 Загрузка данных из файла
        private async Task LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine($"[WhisperService] File not found: {_filePath}");
                    return;
                }

                var json = await File.ReadAllTextAsync(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    _entries.Clear();
                    Console.WriteLine("[WhisperService] File is empty — list cleared.");
                    return;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var list = JsonSerializer.Deserialize<List<WhisperEntry>>(json, options) ?? new List<WhisperEntry>();

                _entries.Clear();
                _entries.AddRange(list);
                Console.WriteLine($"[WhisperService] Loaded {_entries.Count} entries from {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WhisperService] LoadAsync error: {ex.Message}");
            }
        }
    }

    // 📘 Модель одной записи шёпота
    public class WhisperEntry
    {
        public DateTime Time { get; set; }
        public string Hero { get; set; } = string.Empty;
        public string Emotion { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}