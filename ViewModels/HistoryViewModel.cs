// Файл: ViewModels/HistoryViewModel.cs
using MysticWalley.Services;
using System.Collections.ObjectModel; // <-- Важно!
using System.Windows.Input;         // <-- Важно!

namespace MysticWalley.ViewModels
{
    public class HistoryViewModel
    {
        private readonly HistoryService _historyService;

        // 1. Вместо List<T> используем ObservableCollection<T>.
        //    Она автоматически уведомляет UI об изменениях (добавлении/удалении элементов).
        public ObservableCollection<HistoryItem> HistoryItems { get; private set; }

        // 2. Создаем "команды" - это обертка над нашими методами.
        //    К ним мы будем привязываться в XAML.
        public ICommand LoadHistoryCommand { get; }
        public ICommand ClearHistoryCommand { get; }

        public HistoryViewModel(HistoryService historyService)
        {
            _historyService = historyService;
            HistoryItems = new ObservableCollection<HistoryItem>();

            // 3. Привязываем команды к методам.
            LoadHistoryCommand = new Command(async () => await LoadHistoryAsync());
            ClearHistoryCommand = new Command(async () => await ClearHistoryAsync());
        }

        // 4. Логика загрузки, переехавшая из .xaml.cs
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

        // 5. Логика очистки, переехавшая из .xaml.cs
        private async Task ClearHistoryAsync()
        {
            // Здесь можно добавить диалог подтверждения, но для ViewModel лучше делать это
            // на уровне View. Пока оставим так для простоты.
            await _historyService.ClearHistoryAsync();
            HistoryItems.Clear(); // Просто очищаем коллекцию, UI обновится сам.
            Console.WriteLine("[HistoryViewModel] History cleared.");
        }
    }
}