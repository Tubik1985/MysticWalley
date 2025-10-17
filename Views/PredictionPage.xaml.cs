// Файл: Views/PredictionPage.xaml.cs

using MysticWalley.Models;
using MysticWalley.Services;
using System.Text;

namespace MysticWalley.Views;

[QueryProperty(nameof(Character), "Character")]
public partial class PredictionPage : ContentPage
{
    private readonly StoryService _storyService;
    private readonly PredictionService _predictionService;
    private readonly HistoryService _historyService;
    private readonly WhisperService _whisperService;

    public Character Character { get; set; }

    public PredictionPage(
        StoryService storyService,
        PredictionService predictionService,
        HistoryService historyService,
        WhisperService whisperService)
    {
        InitializeComponent();

        _storyService = storyService;
        _predictionService = predictionService;
        _historyService = historyService;
        _whisperService = whisperService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (Character == null)
        {
            // Если персонаж не передан, показываем ошибку и ничего не делаем
            CharacterLabel.Text = "Ошибка";
            CharacterDescription.Text = "Персонаж не найден. Вернитесь на главную.";
            return;
        }

        // Заполняем информацию о персонаже
        CharacterLabel.Text = Character.Name;
        CharacterDescription.Text = Character.Description;
        CharacterIcon.Source = Character.Portrait ?? "placeholder.png";
        BackgroundImage.Source = Character.Background ?? "default_bg.png";

        // Сбрасываем предыдущие результаты
        AiLabel.Text = "";
        SceneLabel.Text = "";
        AiFrame.Opacity = 0;
        SceneFrame.Opacity = 0;
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        // Блокируем кнопку, чтобы избежать двойных нажатий
        PredictButton.IsEnabled = false;

        // --- ЭТАП 1: ЛИЧНОЕ ПРЕДСКАЗАНИЕ (НЕ ЗАВИСИТ ОТ СЦЕНАРИЯ) ---
        try
        {
            // Формируем простой, но контекстный промпт
            var promptForAI = $"Ты {Character.Name}, персонаж из мира MysticWalley. " +
                              $"К тебе пришел путник за советом. " +
                              $"Дай ему короткое, мистическое и личное предсказание, " +
                              $"исходя из твоего характера: {Character.Description}.";

            string aiText = await _predictionService.GetPredictionAsync(promptForAI) ?? "…тишина звёзд…";
            AiLabel.Text = aiText.Trim();

            // Сразу сохраняем в личную историю
            await _historyService.AddAsync(Character.Name, aiText);
            Console.WriteLine($"[PredictionPage] Saved personal prediction for {Character.Name}");

            // Плавно показываем результат
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR during AI prediction: {ex.Message}");
            AiLabel.Text = "Духи сегодня не в настроении говорить...";
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
        }

        // --- ЭТАП 2: ФОНОВЫЙ СЦЕНАРИЙ И ШЁПОТЫ (ОТДЕЛЬНАЯ ЛОГИКА) ---
        try
        {
            if (!_storyService.IsLoaded)
            {
                await _storyService.LoadAsync();
            }

            var episode = _storyService.GetCurrentEpisode();
            if (episode?.Scenes != null && episode.Scenes.Any())
            {
                var reactions = new StringBuilder();
                reactions.AppendLine("Тем временем в долине:");

                foreach (var scene in episode.Scenes)
                {
                    // Сохраняем все сцены в шёпоты
                    await _whisperService.AddImprovisationAsync(scene.HeroId, scene.Emotion, scene.Text);

                    // Формируем текст для отображения
                    reactions.AppendLine($"— {scene.HeroId}: {scene.Text}");
                }

                SceneLabel.Text = reactions.ToString();
                Console.WriteLine($"[PredictionPage] Added {episode.Scenes.Count} scenes to whispers.");

                // Плавно показываем блок со сценами
                await SceneFrame.FadeTo(1, 600, Easing.CubicIn);

                // Переходим к следующему эпизоду только если этот был успешно обработан
                _storyService.GetNextEpisode();
            }
            else
            {
                SceneLabel.Text = "В долине всё спокойно...";
                await SceneFrame.FadeTo(1, 600, Easing.CubicIn);
            }
        }
        catch (Exception ex)
        {
            // ВАЖНО: Ошибка здесь больше не ломает основную функцию предсказания!
            Console.WriteLine($"[PredictionPage] ERROR during scene processing: {ex.Message}");
            SceneLabel.Text = "Эхо долины затихло из-за ошибки...";
            await SceneFrame.FadeTo(1, 600, Easing.CubicIn);
        }
        finally
        {
            // Возвращаем кнопку в рабочее состояние в любом случае
            PredictButton.IsEnabled = true;
        }
    }
}