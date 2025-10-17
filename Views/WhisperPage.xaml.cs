// Файл: Views/WhisperPage.xaml.cs

using MysticWalley.ViewModels;

namespace MysticWalley.Views;

public partial class WhisperPage : ContentPage
{
    // --- ИЗМЕНЕНИЕ 1: Добавляем приватное поле для ViewModel ---
    private readonly WhisperViewModel _viewModel;

    public WhisperPage(WhisperViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.LoadWhispersCommand.CanExecute(null))
        {
            _viewModel.LoadWhispersCommand.Execute(null);
        }
    }

    // --- ИЗМЕНЕНИЕ 2: Добавляем обработчик нажатия кнопки ---
    private void ClearWhispersButton_Clicked(object sender, EventArgs e)
    {
        // Создаем функцию, которая покажет диалог подтверждения
        Func<Task<bool>> askConfirmationFunc = () =>
            DisplayAlert("Подтверждение",
                         "Удалить все шёпоты долины? Это действие необратимо.",
                         "Да, удалить", "Отмена");

        // Вызываем команду из ViewModel, передавая нашу функцию
        if (_viewModel.ClearWhispersCommand.CanExecute(askConfirmationFunc))
        {
            _viewModel.ClearWhispersCommand.Execute(askConfirmationFunc);
        }
    }
}