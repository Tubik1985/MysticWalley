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
}