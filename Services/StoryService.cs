using System.Text.Json;
using MysticWalley.Models;

namespace MysticWalley.Services
{
    public class StoryService
    {
        private StoryRoot? _story;
        private int _currentIndex = 0;

        public bool IsLoaded => _story != null && _story.Episodes?.Any() == true;

        // =========================================================================
        // ИСПРАВЛЕНИЕ (План Б): Загружаем файл напрямую как Content, а не MauiAsset
        // =========================================================================
        public async Task LoadAsync()
        {
            try
            {
                // Формируем прямой путь к файлу в директории, куда он был скопирован при сборке.
                // AppContext.BaseDirectory указывает на папку типа /bin/Debug/net8.0-windows.../
                var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "StoryConfig.json");

                // Добавим проверку на существование файла для более ясной диагностики.
                if (!File.Exists(filePath))
                {
                    var errorMessage = $"[StoryService] CRITICAL ERROR: Файл сценария не найден по прямому пути: {filePath}";
                    Console.WriteLine(errorMessage);
                    // Выбрасываем исключение, чтобы не продолжать работу с пустыми данными.
                    throw new FileNotFoundException(errorMessage, filePath);
                }

                Console.WriteLine($"[StoryService] Читаем файл сценария из: {filePath}");

                // Читаем весь текстовый контент файла асинхронно.
                var json = await File.ReadAllTextAsync(filePath);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _story = JsonSerializer.Deserialize<StoryRoot>(json, options);

                _currentIndex = 0;
                Console.WriteLine($"[StoryService] УСПЕХ: Сценарий '{_story?.Title}' загружен ({_story?.Episodes?.Count ?? 0} эпизодов).");
            }
            catch (Exception ex)
            {
                // Логируем ошибку с указанием её типа для лучшего понимания.
                Console.WriteLine($"[StoryService] ОШИБКА ЗАГРУЗКИ: {ex.GetType().Name} - {ex.Message}");
                // После ошибки сбрасываем состояние, чтобы IsLoaded был false.
                _story = null;
            }
        }

        public Episode? GetCurrentEpisode() =>
            _story?.Episodes?.ElementAtOrDefault(_currentIndex);

        public Episode? GetNextEpisode()
        {
            if (!IsLoaded || _story?.Episodes == null || _story.Episodes.Count == 0)
                return null;

            _currentIndex = (_currentIndex + 1) % _story.Episodes.Count;
            return _story.Episodes[_currentIndex];
        }
    }
}