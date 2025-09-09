using MysticWalley.Models;

namespace MysticWalley.Views;

public partial class PredictionPage : ContentPage
{
    private readonly Character _character;

    public PredictionPage(Character character)
    {
        InitializeComponent();
        _character = character;

        // ��������� ������� �����
        CharacterLabel.Text = _character.Name;
        CharacterIcon.Source = _character.Portrait;
        CharacterDescription.Text = _character.Description;
        BackgroundImage.Source = _character.Background;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // ������� ��������� ���� � ��������
        await BackgroundImage.FadeTo(1, 800, Easing.CubicInOut);
        await CharacterIcon.FadeTo(1, 600, Easing.CubicInOut);
    }

    private async void OnPredictClicked(object sender, EventArgs e)
    {
        string[] phrases =
        {
            "������� ������� ���� ��� ����� ���������.",
            "������ ���� �������� � ��� ���� �� �������.",
            "������ ������� ������, ��� �������.",
            "������� ������ ���������� � ��� ������ �������."
        };

        var rnd = new Random();
        ResultLabel.Text = $"{_character.Name} �������������: \"{phrases[rnd.Next(phrases.Length)]}\"";

        // �������� ���������� ����� ������������
        ResultFrame.Opacity = 0;
        await ResultFrame.FadeTo(1, 600, Easing.CubicInOut);
    }
}