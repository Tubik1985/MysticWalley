// Файл: Views/HistoryPage.xaml.cs

using MysticWalley.ViewModels;

namespace MysticWalley.Views;

public partial class HistoryPage : ContentPage
{
    // --- ИЗМЕНЕНИЕ 1: Добавляем приватное поле для ViewModel ---
    private readonly HistoryViewModel _viewModel;

    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();

        // Сохраняем ViewModel в поле для доступа из других методов
        _viewModel = viewModel;

        // Устанавливаем ViewModel как контекст данных для XAML-привязок
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Используем поле _viewModel для вызова команды
        if (_viewModel.LoadHistoryCommand.CanExecute(null))
        {
            _viewModel.LoadHistoryCommand.Execute(null);
        }
    }

    // --- ИЗМЕНЕНИЕ 2: Добавляем обработчик нажатия кнопки ---
    private void ClearHistoryButton_Clicked(object sender, EventArgs e)
    {
        // 1. Создаем функцию, которая умеет показывать диалог подтверждения.
        //    Эта функция будет передана в ViewModel, которая ничего не знает о DisplayAlert.
        Func<Task<bool>> askConfirmationFunc = () =>
            DisplayAlert("Подтверждение",
                         "Удалить всю историю предсказаний? Это действие необратимо.",
                         "Да, удалить", "Отмена");

        // 2. Проверяем, может ли команда выполниться (хорошая практика).
        if (_viewModel.ClearHistoryCommand.CanExecute(askConfirmationFunc))
        {
            // 3. Выполняем команду, передавая нашу функцию как параметр.
            _viewModel.ClearHistoryCommand.Execute(askConfirmationFunc);
        }
    }
}