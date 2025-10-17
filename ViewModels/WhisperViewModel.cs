// Файл: ViewModels/WhisperViewModel.cs
using MysticWalley.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MysticWalley.ViewModels
{
    public class WhisperViewModel
    {
        private readonly WhisperService _whisperService;

        public ObservableCollection<WhisperEntry> WhisperEntries { get; }
        public ICommand LoadWhispersCommand { get; }

        // =========================================================================
        // ИСПРАВЛЕНИЕ: Команда теперь принимает делегат для подтверждения
        // =========================================================================
        public ICommand ClearWhispersCommand { get; }

        public WhisperViewModel(WhisperService whisperService)
        {
            _whisperService = whisperService;
            WhisperEntries = new ObservableCollection<WhisperEntry>();

            LoadWhispersCommand = new Command(async () => await LoadWhispersAsync());

            // Команда теперь ожидает получить функцию для подтверждения
            ClearWhispersCommand = new Command<Func<Task<bool>>>(async (askConfirmationFunc) =>
            {
                if (askConfirmationFunc == null) return;

                bool userConfirmed = await askConfirmationFunc();

                if (userConfirmed)
                {
                    await ClearWhispersAsync();
                }
            });
        }

        private async Task LoadWhispersAsync()
        {
            await _whisperService.InitializeAsync();
            WhisperEntries.Clear();
            var items = await _whisperService.GetAllAsync();
            foreach (var item in items)
            {
                WhisperEntries.Add(item);
            }
            Console.WriteLine($"[WhisperViewModel] Loaded {WhisperEntries.Count} whisper entries.");
        }

        // Метод очистки стал приватным и простым
        private async Task ClearWhispersAsync()
        {
            await _whisperService.ClearAsync();
            WhisperEntries.Clear();
            Console.WriteLine("[WhisperViewModel] Whispers cleared by user confirmation.");
        }
    }
}