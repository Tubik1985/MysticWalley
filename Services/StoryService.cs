using System.Text.Json;
using MysticWalley.Models;

namespace MysticWalley.Services
{
    public class StoryService
    {
        private StoryRoot? _story;
        private int _currentIndex = 0;

        public bool IsLoaded => _story != null && _story.Episodes?.Any() == true;

        // 💫 Загружаем сценарий из Data/StoryConfig.json (MauiAsset)
        public async Task LoadAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("Data/StoryConfig.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _story = JsonSerializer.Deserialize<StoryRoot>(json, options);

                _currentIndex = 0;
                Console.WriteLine($"[StoryService] JSON loaded: {_story?.Title} ({_story?.Episodes?.Count ?? 0} episodes)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StoryService] Load error: {ex.Message}");
            }
        }

        public Episode? GetCurrentEpisode() =>
            _story?.Episodes?.ElementAtOrDefault(_currentIndex);

        public Episode? GetNextEpisode()
        {
            if (_story?.Episodes == null || _story.Episodes.Count == 0)
                return null;

            _currentIndex = (_currentIndex + 1) % _story.Episodes.Count;
            return _story.Episodes[_currentIndex];
        }
    }
}