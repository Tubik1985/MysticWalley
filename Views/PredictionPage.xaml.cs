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
            // 1️⃣ Загружаем сценарий при необходимости
            if (!_storyService.IsLoaded)
            {
                Console.WriteLine("[PredictionPage] StoryService not loaded — loading JSON...");
                await _storyService.LoadAsync();
            }

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
                    s.HeroId.Equals(Character.HeroId, StringComparison.OrdinalIgnoreCase));

            // 3️⃣ Формируем запрос для ИИ
            var prompt = scene != null
                ? $"{Character.Name}. Эмоция: {scene.Emotion}. {scene.Text}"
                : $"{Character.Name} даёт краткое мистическое предсказание пользователю.";

            Console.WriteLine($"[DEBUG] Prompt >>> {prompt}");

            // 4️⃣ Запрашиваем ответ ИИ
            string aiText = await predictor.GetPredictionAsync(prompt) ?? "…тишина звёзд…";

            // 🔹 обязательно выводим текст на экран
            AiLabel.Text = aiText.Trim();

            // 5️⃣ Сохраняем сюжетную сцену в шёпоты
            if (scene != null)
            {
                var sceneText = $"{scene.HeroId}: {scene.Text}";
                await whispers.AddImprovisationAsync(scene.HeroId, scene.Emotion, sceneText);
            }

            // 6️⃣ Сохраняем импровизацию ИИ того же персонажа
            if (!string.IsNullOrWhiteSpace(aiText))
                await whispers.AddImprovisationAsync(Character.Name, scene?.Emotion ?? "mystery", aiText);

            // 7️⃣ Формируем реплики других героев
            var reactions = new StringBuilder();
            if (episode?.Scenes != null && episode.Scenes.Count > 0)
            {
                foreach (var s in episode.Scenes)
                {
                    var prefix = s.HeroId.Equals(Character.HeroId, StringComparison.OrdinalIgnoreCase) ? "★" : "―";
                    reactions.AppendLine($"{prefix} {s.HeroId}: {s.Text}");
                }
            }
            else
            {
                reactions.AppendLine("Тишина долины. Нет сцен в этом эпизоде.");
            }

            SceneLabel.Text = reactions.ToString();

            // 8️⃣ Анимация появления блоков
            AiFrame.Opacity = 0;
            SceneFrame.Opacity = 0;
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
            await SceneFrame.FadeTo(1, 600, Easing.CubicIn);

            // 9️⃣ Добавляем запись в историю
            await historySvc.AddAsync(Character.Name, AiLabel.Text);

            Console.WriteLine($"[PredictionPage] Saved history + whispers for {Character.Name}");

            // 🔸 следующий эпизод
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