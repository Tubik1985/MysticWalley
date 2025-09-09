using MysticWalley.Models;

namespace MysticWalley.Views;

public partial class PredictionPage : ContentPage
{
    private readonly Character _character;

    public PredictionPage(Character character)
    {
        InitializeComponent();
        _character = character;

        // Заполняем данными героя
        CharacterLabel.Text = _character.Name;
        CharacterIcon.Source = _character.Portrait;
        CharacterDescription.Text = _character.Description;
        BackgroundImage.Source = _character.Background;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Плавное появление фона и портрета
        await BackgroundImage.FadeTo(1, 800, Easing.CubicInOut);
        await CharacterIcon.FadeTo(1, 600, Easing.CubicInOut);
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        string[] phrases =
        {
            "Сегодня хороший день для новых начинаний.",
            "Слушай свою интуицию — она тебя не подведёт.",
            "Мелочи сегодня важнее, чем кажутся.",
            "Избегай пустых разговоров — они крадут энергию."
        };

        var rnd = new Random();
        ResultLabel.Text = $"{_character.Name} предсказывает: \"{phrases[rnd.Next(phrases.Length)]}\"";

        // Анимация проявления блока предсказания
        ResultFrame.Opacity = 0;
        await ResultFrame.FadeTo(1, 600, Easing.CubicInOut);
    }
}