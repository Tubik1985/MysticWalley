// Файл: Models/GameState.cs
namespace MysticWalley.Models
{
    /// <summary>
    /// Хранит глобальное состояние игрового мира,
    /// которое персистентно (сохраняется между сессиями).
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Индекс текущего глобального эпизода из StoryConfig.json.
        /// </summary>
        public int CurrentEpisodeIndex { get; set; } = 0;

        /// <summary>
        /// Словарь для хранения последнего известного настроения персонажей.
        /// Ключ - HeroId, Значение - строка с эмоцией (например, "angry", "gloomy").
        /// </summary>
        public Dictionary<string, string> CharacterMoods { get; set; } = new();
    }
}