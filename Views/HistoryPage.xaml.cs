using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryService _historyService;

    public HistoryPage(HistoryService historyService)
    {
        InitializeComponent();
        _historyService = historyService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        HistoryView.ItemsSource = await _historyService.GetHistoryAsync();
    }
    private async void OnClearHistoryClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert(
            "Подтверждение",
            "Точно очистить всю историю предсказаний?",
            "Да", "Отмена");

        if (confirm)
        {
            await _historyService.ClearHistoryAsync();
            HistoryView.ItemsSource = await _historyService.GetHistoryAsync();
        }
    }
}