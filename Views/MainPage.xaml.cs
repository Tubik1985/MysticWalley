using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class MainPage : ContentPage
{
    private readonly CharacterService _characterService;

    public MainPage(CharacterService characterService)
    {
        InitializeComponent();
        _characterService = characterService;
        CharactersView.ItemsSource = _characterService.GetCharacters();
    }

    // переход к выбранному герою
    private async void OnCharacterSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Character selected)
        {
            CharactersView.SelectedItem = null;
            await Shell.Current.GoToAsync(nameof(PredictionPage), true,
                new Dictionary<string, object> { { "Character", selected } });
        }
    }

    // переход на страницу истории предсказаний
    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    // переход на страницу «Шёпот Долины»
    private async void OnWhisperClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WhisperPage));
    }
}