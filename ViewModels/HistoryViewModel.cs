// Файл: ViewModels/HistoryViewModel.cs

using MysticWalley.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MysticWalley.ViewModels
{
    public class HistoryViewModel
    {
        private readonly HistoryService _historyService;

        public ObservableCollection<HistoryItem> HistoryItems { get; }
        public ICommand LoadHistoryCommand { get; }

        // =========================================================================
        // ИСПРАВЛЕНИЕ: Команда теперь принимает делегат для подтверждения
        // =========================================================================

        /// <summary>
        /// Команда для очистки истории.
        /// Ожидает в качестве параметра Func<Task<bool>> - функцию, 
        /// которая покажет диалог подтверждения и вернет true или false.
        /// </summary>
        public ICommand ClearHistoryCommand { get; }

        public HistoryViewModel(HistoryService historyService)
        {
            _historyService = historyService;
            HistoryItems = new ObservableCollection<HistoryItem>();

            LoadHistoryCommand = new Command(async () => await LoadHistoryAsync());

            // Команда теперь типизирована как Command<T>, где T - это Func<Task<bool>>.
            // Это позволяет нам передать функцию подтверждения из View.
            ClearHistoryCommand = new Command<Func<Task<bool>>>(async (askConfirmationFunc) =>
            {
                // Если по какой-то причине функция не была передана,
                // мы просто не будем выполнять очистку, чтобы избежать случайных удалений.
                if (askConfirmationFunc == null) return;

                // Вызываем сам диалог, который реализован во View
                bool userConfirmed = await askConfirmationFunc();

                // Действуем только если пользователь нажал "Да"
                if (userConfirmed)
                {
                    await ClearHistoryAsync();
                }
            });
        }

        private async Task LoadHistoryAsync()
        {
            HistoryItems.Clear();
            var items = await _historyService.GetAllAsync();
            foreach (var item in items)
            {
                HistoryItems.Add(item);
            }
            Console.WriteLine($"[HistoryViewModel] Loaded {HistoryItems.Count} items.");
        }

        // Метод очистки теперь стал приватным и более простым.
        // Вся логика подтверждения вынесена в саму команду.
        private async Task ClearHistoryAsync()
        {
            await _historyService.ClearHistoryAsync();
            HistoryItems.Clear();
            Console.WriteLine("[HistoryViewModel] History cleared by user confirmation.");
        }
    }
}