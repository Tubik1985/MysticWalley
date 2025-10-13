using System.Text.Json;

namespace MysticWalley.Services
{
    public class HistoryService
    {
        private readonly string _filePath =
            Path.Combine(FileSystem.AppDataDirectory, "history.json");

        private readonly List<HistoryItem> _items = new();

        // Добавление нового предсказания в историю
        public async Task AddAsync(string hero, string aiText)
        {
            await LoadAsync();

            _items.Add(new HistoryItem
            {
                Time = DateTime.Now,
                Hero = hero,
                Text = aiText
            });

            await SaveAsync();
        }

        // Получить все записи истории
        public async Task<List<HistoryItem>> GetAllAsync()
        {
            await LoadAsync();
            // Новые сверху
            return _items.OrderByDescending(i => i.Time).ToList();
        }

        // Очистить историю
        public async Task ClearHistoryAsync()
        {
            try
            {
                _items.Clear();
                if (File.Exists(_filePath))
                    File.Delete(_filePath);

                await Task.CompletedTask;
                Console.WriteLine("[HistoryService] История очищена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HistoryService] Clear error: {ex.Message}");
            }
        }

        // ───── служебные методы сохранения и загрузки ─────

        private async Task SaveAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(
                    _items, new JsonSerializerOptions { WriteIndented = true });

                await File.WriteAllTextAsync(_filePath, json);
                Console.WriteLine($"[HistoryService] Saved {_items.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HistoryService] Save error: {ex.Message}");
            }
        }

        private async Task LoadAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return;

                var json = await File.ReadAllTextAsync(_filePath);
                var list = JsonSerializer.Deserialize<List<HistoryItem>>(json);

                _items.Clear();
                if (list != null)
                    _items.AddRange(list);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HistoryService] Load error: {ex.Message}");
            }
        }
    }

    // Модель одной записи истории
    public class HistoryItem
    {
        public DateTime Time { get; set; }
        public string Hero { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}