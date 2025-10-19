// Файл: Views/PredictionPage.xaml.cs

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using MysticWalley.Models;
using MysticWalley.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysticWalley.Views;

[QueryProperty(nameof(CharacterId), "characterId")]
public partial class PredictionPage : ContentPage
{
    // ================================================================
    // ОБЪЯВЛЕНИЕ ВСЕХ ПОЛЕЙ КЛАССА
    // ================================================================
    private readonly CharacterService _characterService;
    private readonly GameStateService _gameStateService;
    private readonly StoryService _storyService;
    private readonly PredictionService _predictionService;
    private readonly HistoryService _historyService;
    private readonly WhisperService _whisperService;

    public string CharacterId { get; set; }
    private Character? _character;
    // ================================================================


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

        AiLabel.Text = "";
        SceneContainer.Clear(); // Очищаем контейнер сцен
        AiFrame.Opacity = 0;
        PredictButton.IsEnabled = true;

        AiFrame.TranslationY = 50;
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
        SceneContainer.Clear();
        // --- ЭТАП 1: ЛИЧНОЕ ПРЕДСКАЗАНИЕ ---
        try
        {
            var promptForAI = $"Ты {_character.Name}, персонаж мира MysticWalley. Твой характер: {_character.Description}. Дай путнику короткое, мистическое предсказание.";
            string? aiText = await _predictionService.GetPredictionAsync(promptForAI) ?? "…тишина звёзд…";
            AiLabel.Text = aiText.Trim();
            await _historyService.AddAsync(_character.Name, aiText);

            await Task.WhenAll(
                AiFrame.FadeTo(1, 400, Easing.CubicIn),
                AiFrame.TranslateTo(0, 0, 400, Easing.CubicOut)
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR during AI prediction: {ex.Message}");
            AiLabel.Text = "Духи сегодня не в настроении говорить...";
            await AiFrame.FadeTo(1, 400, Easing.CubicIn);
        }

        await Task.Delay(500);

        // --- ЭТАП 2: ПОТОК СЕРИАЛА ---
        await PlayStoryEpisode();
    }

    private async Task PlayStoryEpisode()
    {
        try
        {
            var episode = _storyService.GetCurrentEpisode();
            if (episode?.Scenes == null || !episode.Scenes.Any())
            {
                var quietBorder = CreateSceneBorder("В долине всё спокойно...", Colors.DimGray);
                SceneContainer.Add(quietBorder);
                await AnimateViewIn(quietBorder, true);
                return;
            }

            var newMoods = new Dictionary<string, string>();
            bool slideFromLeft = true;

            foreach (var scene in episode.Scenes)
            {
                var characterForScene = _characterService.GetCharacterById(scene.HeroId);
                if (characterForScene == null) continue;

                var improvPrompt = $"Ты персонаж по имени {characterForScene.Name}. Характер: {characterForScene.Description}. Твоя эмоция сейчас: '{scene.Emotion}'. Твоя ключевая мысль для этой сцены: '{scene.Text}'. ЗАДАЧА: Не повторяй эту мысль дословно. Сымпровизируй на ее основе короткую, загадочную реплику в своем стиле.";
                var improvisedText = await _predictionService.GetImprovisedLineAsync(improvPrompt) ?? scene.Text;

                await _whisperService.AddImprovisationAsync(scene.HeroId, scene.Emotion, improvisedText.Trim());

                var sceneText = $"— {characterForScene.Name}: {improvisedText.Trim()}";
                var emotionColor = GetColorForEmotion(scene.Emotion);

                var sceneBorder = CreateSceneBorder(sceneText, emotionColor);
                SceneContainer.Add(sceneBorder);
                await AnimateViewIn(sceneBorder, slideFromLeft);

                newMoods[scene.HeroId] = scene.Emotion;
                slideFromLeft = !slideFromLeft;
                await Task.Delay(400);
            }

            await _storyService.AdvanceToNextEpisodeAsync();
            await _gameStateService.UpdateAndSaveStateAsync(state =>
            {
                foreach (var mood in newMoods) state.CharacterMoods[mood.Key] = mood.Value;
            });
            UpdateDebugInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PredictionPage] ERROR during scene processing: {ex.Message}");
        }
        finally
        {
            await Task.Delay(700);
            PredictButton.IsEnabled = true;
        }
    }

    private Border CreateSceneBorder(string text, Color borderColor)
    {
        var border = new Border
        {
            Padding = 14,
            BackgroundColor = Color.FromArgb("#80202020"),
            Stroke = borderColor,
            StrokeThickness = 2,
            Opacity = 0
        };
        border.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(14) };
        border.Content = new Label
        {
            Text = text,
            FontSize = 16,
            TextColor = Color.FromArgb("#EAEAEA"),
            HorizontalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.WordWrap
        };
        return border;
    }

    private async Task AnimateViewIn(View view, bool fromLeft)
    {
        double startX = fromLeft ? -this.Width : this.Width;
        if (Math.Abs(startX) < 100) startX = fromLeft ? -400 : 400;
        view.TranslationX = startX;

        await Task.WhenAll(
            view.FadeTo(1, 600, Easing.SinOut),
            view.TranslateTo(0, 0, 600, Easing.SinOut)
        );
    }

    private Color GetColorForEmotion(string? emotion)
    {
        return emotion?.ToLowerInvariant() switch
        {
            "angry" or "impatient" or "challenging" => Color.FromRgb(139, 0, 0),
            "alarmed" or "worried" or "annoyed" or "dismissive" => Color.FromRgb(72, 61, 139),
            "knowing" or "smug" or "revelatory" => Color.FromRgb(75, 0, 130),
            "playful" or "amused" or "intrigued" => Color.FromRgb(184, 134, 11),
            "thoughtful" or "serene" or "calm" => Color.FromRgb(0, 100, 0),
            "ecstatic" => Color.FromRgb(128, 0, 128),
            _ => Colors.DimGray
        };
    }

    private void ShowError(string title, string message)
    {
        CharacterLabel.Text = title;
        CharacterDescription.Text = message;
    }
}