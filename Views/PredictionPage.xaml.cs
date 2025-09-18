using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

[QueryProperty(nameof(Character), "Character")]
public partial class PredictionPage : ContentPage
{
    private readonly PredictionService _predictionService;
    private readonly HistoryService _historyService;

    public Character? Character { get; set; }

    public PredictionPage(PredictionService predictionService, HistoryService historyService)
    {
        InitializeComponent();
        _predictionService = predictionService;
        _historyService = historyService;
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

            await BackgroundImage.FadeTo(1, 800, Easing.CubicInOut);
            await CharacterIcon.FadeTo(1, 600, Easing.CubicInOut);
        }
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        if (Character == null) return;

        var result = await _predictionService.GetPredictionAsync(Character.Name);

        ResultLabel.Text = result;

        await _historyService.SaveMessageAsync(new ChatMessage
        {
            UserMessage = "Предсказание",
            Response = result,
            Character = Character.Name,
            Timestamp = DateTime.Now
        });

        ResultFrame.Opacity = 0;
        await ResultFrame.FadeTo(1, 600, Easing.CubicInOut);
    }
}