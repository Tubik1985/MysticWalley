// Файл: Views/PredictionPage.xaml.cs

using MysticWalley.Models;
using MysticWalley.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysticWalley.Views;

[QueryProperty(nameof(CharacterId), "characterId")]
public partial class PredictionPage : ContentPage
{
    private readonly CharacterService _characterService;
    private readonly GameStateService _gameStateService;
    private readonly StoryService _storyService;
    private readonly PredictionService _predictionService;
    private readonly HistoryService _historyService;
    private readonly WhisperService _whisperService;

    public string CharacterId { get; set; }
    private Character? _character;

    public PredictionPage(
        CharacterService characterService,
        GameStateService gameStateService,
        StoryService storyService,
        PredictionService predictionService,
        HistoryService historyService,
        WhisperService whisperService)
    {
        InitializeComponent();
        _characterService = characterService;
        _gameStateService = gameStateService;
        _storyService = storyService;
        _predictionService = predictionService;
        _historyService = historyService;
        _whisperService = whisperService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("\n--- [PredictionPage] OnAppearing START ---");
        if (string.IsNullOrEmpty(CharacterId)) { ShowError("Ошибка", "CharacterId не был передан."); return; }
        _character = _characterService.GetCharacterById(CharacterId);
        if (_character == null) { ShowError("Ошибка", $"Персонаж с ID {CharacterId} не найден."); return; }

        UpdateCharacterUI();

        if (!_storyService.IsLoaded)
        {
            await _storyService.LoadAsync();
        }
        UpdateDebugInfo();
        Console.WriteLine("--- [PredictionPage] OnAppearing END (SUCCESS) ---\n");
    }

    private void UpdateCharacterUI()
    {
        if (_character == null) return;
        CharacterLabel.Text = _character.Name;
        CharacterDescription.Text = _character.Description;
        CharacterIcon.Source = _character.Portrait;
        BackgroundImage.Source = _character.Background;
        BackgroundImage.FadeTo(0.6, 800, Easing.CubicInOut);
        AiLabel.Text = ""; SceneLabel.Text = "";
        AiFrame.Opacity = 0; SceneFrame.Opacity = 0;
        PredictButton.IsEnabled = true;
    }

    private void UpdateDebugInfo()
    {
        var episode = _storyService.GetCurrentEpisode();
        DebugEpisodeLabel.Text = $"Текущий эпизод: {episode?.Id ?? "СЦЕНАРИЙ ЗАВЕРШЕН"}";
        var currentState = _gameStateService.GetCurrentState();
        var moodsText = string.Join(", ", currentState.CharacterMoods.Select(kv => $"{kv.Key}: {kv.Value}"));
        DebugMoodsLabel.Text = $"Настроения: {(string.IsNullOrEmpty(moodsText) ? "(пусто)" : moodsText)}";
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        if (_character == null) return;
        PredictButton.IsEnabled = false;

        // --- ЭТАП 1: ЛИЧНОЕ ПРЕДСКАЗАНИЕ ---
        try
        {
            var promptForAI = $"Ты {_character.Name}, персонаж мира MysticWalley. Твой характер: {_character.Description}. Дай путнику короткое, мистическое предсказание.";
            string? aiText = await _predictionService.GetPredictionAsync(promptForAI) ?? "…тишина звёзд…";
            AiLabel.Text = aiText.Trim();
            await _historyService.AddAsync(_character.Name, aiText);
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR during AI prediction: {ex.Message}");
            AiLabel.Text = "Духи сегодня не в настроении говорить...";
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
        }

        // --- ЭТАП 2: ПОТОК СЕРИАЛА ---
        try
        {
            var episode = _storyService.GetCurrentEpisode();

            if (episode?.Scenes != null && episode.Scenes.Any())
            {
                var reactions = new StringBuilder();
                var newMoods = new Dictionary<string, string>();

                reactions.AppendLine("Тем временем в долине:");

                // =========================================================================
                // ГЛАВНОЕ ИЗМЕНЕНИЕ: Заменяем "заглушку" на реальную импровизацию.
                // =========================================================================
                foreach (var scene in episode.Scenes)
                {
                    var characterForScene = _characterService.GetCharacterById(scene.HeroId);
                    if (characterForScene == null) continue;

                    // Формируем "промпт-режиссера"
                    var improvPrompt =
                        $"Ты персонаж по имени {characterForScene.Name}. Характер: {characterForScene.Description}. " +
                        $"Твоя эмоция сейчас: '{scene.Emotion}'. " +
                        $"Твоя ключевая мысль для этой сцены: '{scene.Text}'. " +
                        $"ЗАДАЧА: Не повторяй эту мысль дословно. Сымпровизируй на ее основе короткую, загадочную реплику в своем стиле.";

                    // Вызываем новый метод для импровизации
                    var improvisedText = await _predictionService.GetImprovisedLineAsync(improvPrompt) ?? scene.Text;

                    // Сохраняем и отображаем УНИКАЛЬНЫЙ, сгенерированный текст
                    await _whisperService.AddImprovisationAsync(scene.HeroId, scene.Emotion, improvisedText.Trim());
                    reactions.AppendLine($"— {characterForScene.Name}: {improvisedText.Trim()}");

                    newMoods[scene.HeroId] = scene.Emotion;
                }
                // =========================================================================

                SceneLabel.Text = reactions.ToString();
                await SceneFrame.FadeTo(1, 600, Easing.CubicIn);

                await _storyService.AdvanceToNextEpisodeAsync();
                await _gameStateService.UpdateAndSaveStateAsync(state =>
                {
                    foreach (var mood in newMoods) state.CharacterMoods[mood.Key] = mood.Value;
                });

                UpdateDebugInfo();
            }
            else
            {
                SceneLabel.Text = "В долине всё спокойно...";
                await SceneFrame.FadeTo(1, 600, Easing.CubicIn);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR during scene processing: {ex.Message}");
            SceneLabel.Text = "Эхо долины затихло из-за ошибки...";
            await SceneFrame.FadeTo(1, 600, Easing.CubicIn);
        }
        finally
        {
            await Task.Delay(700);
            PredictButton.IsEnabled = true;
        }
    }

    private void ShowError(string title, string message)
    {
        CharacterLabel.Text = title;
        CharacterDescription.Text = message;
    }
}