// Файл: Views/MainPage.xaml.cs

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

    private async void OnCharacterSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Character selected)
        {
            CharactersView.SelectedItem = null;

            // =========================================================================
            // ИЗМЕНЕНИЕ: Передаем только ID персонажа в виде простого строкового маршрута.
            // Это самый надежный способ навигации в MAUI Shell.
            // =========================================================================
            var route = $"{nameof(PredictionPage)}?characterId={selected.HeroId}";
            await Shell.Current.GoToAsync(route);
        }
    }

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HistoryPage));
    }

    private async void OnWhisperClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WhisperPage));
    }
}