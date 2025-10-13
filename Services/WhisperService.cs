using System.Text.Json;

namespace MysticWalley.Services
{
    public class WhisperService
    {
        private readonly string _filePath =
            Path.Combine(FileSystem.AppDataDirectory, "whispers.json");

        private readonly List<WhisperEntry> _entries = new();

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

        public async Task<IEnumerable<WhisperEntry>> GetAllAsync()
        {
            await LoadAsync();
            return _entries.OrderByDescending(x => x.Time);
        }

        public async Task AddImprovisationAsync(string hero, string emotion, string text)
        {
            try
            {
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

        public async Task ClearAsync()
        {
            _entries.Clear();
            if (File.Exists(_filePath))
                File.Delete(_filePath);

            await Task.CompletedTask;
        }

        private async Task LoadAsync()
        {
            if (!File.Exists(_filePath))
                return;

            var json = await File.ReadAllTextAsync(_filePath);
            var list = JsonSerializer.Deserialize<List<WhisperEntry>>(json);

            _entries.Clear();
            if (list != null)
                _entries.AddRange(list);
        }
    }

    public class WhisperEntry
    {
        public DateTime Time { get; set; }
        public string Hero { get; set; } = string.Empty;
        public string Emotion { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}