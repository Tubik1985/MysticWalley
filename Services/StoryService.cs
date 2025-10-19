// Файл: Services/StoryService.cs
using MysticWalley.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace MysticWalley.Services
{
    public class StoryService
    {
        // --- ИЗМЕНЕНИЕ №1: Добавляем зависимость от GameStateService ---
        private readonly GameStateService _gameStateService;

        private StoryRoot? _story;
        // --- ИЗМЕНЕНИЕ №2: УДАЛЯЕМ внутреннее состояние _currentIndex ---
        // private int _currentIndex = 0; // Больше не нужно!

        public bool IsLoaded => _story != null && _story.Episodes?.Any() == true;

        // Внедряем GameStateService через конструктор
        public StoryService(GameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        // Метод загрузки сценария остается почти без изменений
        public async Task LoadAsync()
        {
            if (IsLoaded) return; // Не загружаем повторно, если уже загружен

            try
            {
                // Используем наш "План Б" для загрузки файла
                var filePath = Path.Combine(AppContext.BaseDirectory, "Resources", "Data", "StoryConfig.json");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[StoryService] CRITICAL ERROR: Файл сценария не найден: {filePath}");
                    return;
                }

                var json = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _story = JsonSerializer.Deserialize<StoryRoot>(json, options);

                Console.WriteLine($"[StoryService] УСПЕХ: Сценарий '{_story?.Title}' загружен ({_story?.Episodes?.Count ?? 0} эпизодов).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService] ОШИБКА ЗАГРУЗКИ СЦЕНАРИЯ: {ex.Message}");
            }
        }

        /// <summary>
        /// Возвращает текущий эпизод, основываясь на ГЛОБАЛЬНОМ состоянии игры.
        /// </summary>
        public Episode? GetCurrentEpisode()
        {
            if (!IsLoaded) return null;

            // --- ИЗМЕНЕНИЕ №3: Получаем индекс из GameStateService ---
            var state = _gameStateService.GetCurrentState();
            int currentIndex = state.CurrentEpisodeIndex;

            return _story?.Episodes?.ElementAtOrDefault(currentIndex);
        }

        /// <summary>
        /// Готовит мир к следующему эпизоду.
        /// ВАЖНО: Этот метод больше не возвращает эпизод, он только обновляет состояние.
        /// </summary>
        public async Task AdvanceToNextEpisodeAsync()
        {
            if (!IsLoaded || _story?.Episodes == null || !_story.Episodes.Any())
                return;

            var state = _gameStateService.GetCurrentState();
            int currentIndex = state.CurrentEpisodeIndex;

            // --- ИЗМЕНЕНИЕ №4: Увеличиваем индекс и сохраняем его через GameStateService ---
            int nextIndex = (currentIndex + 1) % _story.Episodes.Count;

            await _gameStateService.UpdateAndSaveStateAsync(s =>
            {
                s.CurrentEpisodeIndex = nextIndex;
                // В будущем здесь же будет обновляться настроение персонажей
            });

            Console.WriteLine($"[StoryService] Игровой мир переведен на следующий эпизод, индекс: {nextIndex}");
        }
    }
}