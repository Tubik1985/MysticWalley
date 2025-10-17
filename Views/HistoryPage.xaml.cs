// Файл: Views/HistoryPage.xaml.cs

using MysticWalley.ViewModels; // Подключаем нашу ViewModel

namespace MysticWalley.Views;

public partial class HistoryPage : ContentPage
{
    // 1. Получаем в конструктор уже готовую ViewModel.
    //    DI-контейнер создаст её для нас вместе с HistoryService внутри.
    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();

        // 2. Устанавливаем ViewModel как контекст данных для этой страницы.
        //    Теперь весь XAML будет "видеть" публичные свойства и команды из viewModel.
        BindingContext = viewModel;
    }

    // 3. Вызываем команду загрузки истории при появлении страницы.
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Мы могли бы сделать это и через XAML (с помощью Behaviors),
        // но для простоты начнем с вызова команды из Code-behind.
        if (BindingContext is HistoryViewModel vm && vm.LoadHistoryCommand.CanExecute(null))
        {
            vm.LoadHistoryCommand.Execute(null);
        }
    }
}