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
            // Снимаем выделение, иначе элемент не «кликабелен» повторно
            CharactersView.SelectedItem = null;

            // Временно просто алерт с выбранным именем
            await DisplayAlert("Выбор героя", $"Вы выбрали: {selected.Name}", "OK");
        }
    }
}