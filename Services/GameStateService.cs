// Файл: Services/GameStateService.cs
using MysticWalley.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace MysticWalley.Services
{
    /// <summary>
    /// Управляет сохранением и загрузкой глобального состояния игры (GameState).
    /// </summary>
    public class GameStateService
    {
        private readonly string _filePath;
        private GameState _currentState;

        public GameStateService()
        {
            _filePath = Path.Combine(FileSystem.AppDataDirectory, "gamestate.json");
            _currentState = new GameState();
        }

        /// <summary>
        /// Загружает состояние игры из файла или создает новый, если он не существует.
        /// Должен вызываться один раз при старте приложения.
        /// </summary>
        public async Task LoadStateAsync()
        {
            if (!File.Exists(_filePath))
            {
                await SaveStateAsync(); // Создаем файл с состоянием по умолчанию
                Console.WriteLine($"[GameStateService] Новый файл состояния создан по пути: {_filePath}");
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_filePath);
                _currentState = JsonSerializer.Deserialize<GameState>(json) ?? new GameState();
                Console.WriteLine($"[GameStateService] Состояние загружено. Текущий эпизод: {_currentState.CurrentEpisodeIndex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameStateService] Ошибка загрузки состояния: {ex.Message}. Используется состояние по умолчанию.");
                _currentState = new GameState(); // В случае ошибки используем чистое состояние
            }
        }

        /// <summary>
        /// Сохраняет текущее состояние игры в файл.
        /// </summary>
        public async Task SaveStateAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_currentState, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
                Console.WriteLine($"[GameStateService] Состояние сохранено.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GameStateService] Ошибка сохранения состояния: {ex.Message}");
            }
        }

        /// <summary>
        /// Возвращает текущее состояние игры.
        /// </summary>
        public GameState GetCurrentState()
        {
            return _currentState;
        }

        /// <summary>
        /// Обновляет состояние игры извне и немедленно сохраняет его.
        /// </summary>
        public async Task UpdateAndSaveStateAsync(Action<GameState> updateAction)
        {
            updateAction(_currentState);
            await SaveStateAsync();
        }
    }
}