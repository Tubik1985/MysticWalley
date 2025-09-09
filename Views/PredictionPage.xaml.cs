using MysticWalley.Models;

namespace MysticWalley.Views;

public partial class PredictionPage : ContentPage
{
    private readonly Character _character;

    public PredictionPage(Character character)
    {
        InitializeComponent();
        _character = character;

        // Заполняем из XAML поля
        CharacterLabel.Text = _character.Name;
        CharacterIcon.Source = _character.Portrait;
        CharacterDescription.Text = _character.Description;
    }

    private void OnPredictClicked(object sender, EventArgs e)
    {
        string[] phrases =
        {
            "Сегодня хороший день для начала новых дел.",
            "В мелочах кроется большая удача.",
            "Не давайте пустых обещаний.",
            "Лучше держать эмоции под контролем."
        };

        var rnd = new Random();
        ResultLabel.Text = $"{_character.Name} предсказывает: \"{phrases[rnd.Next(phrases.Length)]}\"";
    }
}