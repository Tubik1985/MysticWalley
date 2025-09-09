using MysticWalley.Models;

namespace MysticWalley.Views;

public partial class PredictionPage : ContentPage
{
    private readonly Character _character; // выбранный герой

    // Конструктор страницы — принимает выбранного героя
    public PredictionPage(Character character)
    {
        InitializeComponent();
        _character = character;
        CharacterLabel.Text = $"Вы зашли к: {_character.Name}";
    }

    // Обработчик нажатия кнопки "Предсказать"
    private void OnPredictClicked(object sender, EventArgs e)
    {
        string[] phrases =
        {
            "Сегодня хороший день.",
            "Будьте внимательны к мелочам.",
            "Избегайте пустых решений на эмоциях."
        };

        var rnd = new Random();
        ResultLabel.Text = $"{_character.Name} говорит: \"{phrases[rnd.Next(phrases.Length)]}\"";
    }
}