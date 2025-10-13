using System.Text;
using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

[QueryProperty(nameof(Character), "Character")]
public partial class PredictionPage : ContentPage
{
    private readonly StoryService _storyService;
    public Character Character { get; set; }

    public PredictionPage(StoryService storyService)
    {
        InitializeComponent();
        _storyService = storyService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Character == null) return;

        CharacterLabel.Text = Character.Name;
        CharacterDescription.Text = Character.Description;
        CharacterIcon.Source = Character.Portrait ?? "placeholder.png";
        BackgroundImage.Source = Character.Background ?? "default_bg.png";

        await BackgroundImage.FadeTo(0.6, 800, Easing.CubicInOut);
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        try
        {
            // 1️⃣ Загружаем сценарий если ещё не подгружен
            if (!_storyService.IsLoaded)
            {
                Console.WriteLine("[PredictionPage] StoryService not loaded — loading JSON...");
                await _storyService.LoadAsync();
            }

            // Получаем текущий эпизод и необходимые сервисы
            var episode = _storyService.GetCurrentEpisode();
            var predictor = App.Services.GetService<PredictionService>();
            var historySvc = App.Services.GetService<HistoryService>();
            var whispers = App.Services.GetService<WhisperService>();

            if (predictor == null || historySvc == null || whispers == null)
            {
                Console.WriteLine("[PredictionPage] One or more services not resolved.");
                return;
            }

            // 2️⃣ Находим сцену выбранного героя
            Scene? scene = episode?.Scenes?
                .FirstOrDefault(s =>
                    s.HeroId.Equals(Character.HeroId, StringComparison.OrdinalIgnoreCase))
                ?? episode?.Scenes?.FirstOrDefault();

            if (scene == null)
            {
                AiLabel.Text = "Долина молчит. Нет сцен в этом эпизоде.";
                SceneLabel.Text = string.Empty;
                return;
            }

            // 3️⃣ Формируем промпт и запрашиваем ответ у ИИ
            var prompt = $"{Character.Name}. Эмоция: {scene.Emotion}. {scene.Text}";
            Console.WriteLine($"[DEBUG] Prompt >>> {prompt}");

            string aiText = await predictor.GetPredictionAsync(prompt) ?? "…тишина звёзд…";
            AiLabel.Text = aiText.Trim();

            // 4️⃣ Сохраняем ВСЕ сцены эпизода в шёпоты
            if (episode?.Scenes != null && episode.Scenes.Count > 0)
            {
                foreach (var s in episode.Scenes)
                {
                    var record = $"{s.HeroId}: {s.Text}";
                    await whispers.AddImprovisationAsync(s.HeroId, s.Emotion, record);
                }
                Console.WriteLine($"[PredictionPage] Added {episode.Scenes.Count} scenes of episode {episode.Id} to whispers.");
            }
            else
            {
                await whispers.AddImprovisationAsync(Character.HeroId, scene.Emotion, scene.Text);
            }

            // ⚠️ Личное предсказание ИИ оставляем только в истории, не пишем в шёпоты

            // 5️⃣ Формируем текст реплик для отображения на экране
            var reactions = new StringBuilder();
            if (episode?.Scenes != null && episode.Scenes.Count > 0)
            {
                foreach (var s in episode.Scenes)
                {
                    var prefix = s.HeroId.Equals(Character.HeroId, StringComparison.OrdinalIgnoreCase) ? "★" : "—";
                    reactions.AppendLine($"{prefix} {s.HeroId}: {s.Text}");
                }
            }
            else
            {
                reactions.AppendLine("Тишина долины. Нет сцен в этом эпизоде.");
            }

            SceneLabel.Text = reactions.ToString();

            // 6️⃣ Плавная анимация появления блоков
            AiFrame.Opacity = 0;
            SceneFrame.Opacity = 0;
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
            await SceneFrame.FadeTo(1, 600, Easing.CubicIn);

            // 7️⃣ Сохраняем личное предсказание в историю пользователя
            await historySvc.AddAsync(Character.Name, AiLabel.Text);
            Console.WriteLine($"[PredictionPage] Saved personal prediction for {Character.Name}");

            // 8️⃣ Переходим к следующему эпизоду
            _storyService.GetNextEpisode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR: {ex.Message}");
            AiLabel.Text = "Ошибка загрузки предсказания.";
            SceneLabel.Text = string.Empty;
            AiFrame.Opacity = SceneFrame.Opacity = 1;
        }
    }
}