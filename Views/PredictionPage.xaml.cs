using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

[QueryProperty(nameof(Character), "Character")]
public partial class PredictionPage : ContentPage
{
    private readonly PredictionService _predictionService;

    public Character? Character { get; set; }

    public PredictionPage(PredictionService predictionService)
    {
        InitializeComponent();
        _predictionService = predictionService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (Character != null)
        {
            CharacterLabel.Text = Character.Name;
            CharacterIcon.Source = Character.Portrait;
            CharacterDescription.Text = Character.Description;
            BackgroundImage.Source = Character.Background;
        }

        // Анимация плавного проявления
        await BackgroundImage.FadeTo(1, 800, Easing.CubicInOut);
        await CharacterIcon.FadeTo(1, 600, Easing.CubicInOut);
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        if (Character != null)
        {
            var result = await _predictionService.GetPredictionAsync(Character.Name);
            ResultLabel.Text = result;

            ResultFrame.Opacity = 0;
            await ResultFrame.FadeTo(1, 600, Easing.CubicInOut);
        }
    }
}