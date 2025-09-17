using MysticWalley.Services;

namespace MysticWalley.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryService _historyService;

    public HistoryPage()
    {
        InitializeComponent();
        _historyService = new HistoryService(); // временно свой инстанс
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var messages = await _historyService.GetHistoryAsync();
        HistoryList.ItemsSource = messages.OrderByDescending(m => m.Timestamp);
    }
}