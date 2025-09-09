using MysticWalley.Models;

namespace MysticWalley.Views;

public partial class PredictionPage : ContentPage
{
    private readonly Character _character;

    public PredictionPage(Character character)
    {
        InitializeComponent();
        _character = character;

        // ��������� �� XAML ����
        CharacterLabel.Text = _character.Name;
        CharacterIcon.Source = _character.Portrait;
        CharacterDescription.Text = _character.Description;
    }

    private void OnPredictClicked(object sender, EventArgs e)
    {
        string[] phrases =
        {
            "������� ������� ���� ��� ������ ����� ���.",
            "� ������� ������� ������� �����.",
            "�� ������� ������ ��������.",
            "����� ������� ������ ��� ���������."
        };

        var rnd = new Random();
        ResultLabel.Text = $"{_character.Name} �������������: \"{phrases[rnd.Next(phrases.Length)]}\"";
    }
}