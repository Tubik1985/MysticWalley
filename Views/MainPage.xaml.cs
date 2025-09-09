using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        // Привязываем к списку персонажей
        CharactersView.ItemsSource = CharacterService.GetCharacters();
    }

    private async void OnCharacterSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Character selected)
        {
            CharactersView.SelectedItem = null;

            // было: Alert, теперь переход
            await Navigation.PushAsync(new PredictionPage(selected));
        }
    }
}
