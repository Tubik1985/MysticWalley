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

    // 🔹 Загружаем записи при каждом появлении страницы
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var items = await _historyService.GetAllAsync(); // ← верное поле и асинхронный метод
        HistoryView.ItemsSource = items;
    }

    // 🔹 Очистка истории по кнопке
    private async void OnClearHistoryClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Подтверждение",
            "Очистить всю историю предсказаний?",
            "Да", "Отмена");

        if (!confirm)
            return;

        await _historyService.ClearHistoryAsync();

        // после очистки читаем список заново — теперь он будет пуст
        var items = await _historyService.GetAllAsync();
        HistoryView.ItemsSource = items;
    }
}