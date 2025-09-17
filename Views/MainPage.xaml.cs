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

            // Навигация по Shell + передаём объект
            await Shell.Current.GoToAsync(nameof(PredictionPage), true,
                new Dictionary<string, object>
                {
                    { "Character", selected }
                });
        }
    }
}