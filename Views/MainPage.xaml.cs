using MysticWalley.Models;
using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        CharactersView.ItemsSource = CharacterService.GetCharacters();
    }

    private async void OnCharacterSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Character selected)
        {
            CharactersView.SelectedItem = null;

            // Лёгкий fade переход
            await this.FadeTo(0.8, 150, Easing.CubicInOut);
            await Navigation.PushAsync(new PredictionPage(selected));
            await this.FadeTo(1, 150, Easing.CubicInOut);
        }
    }
}