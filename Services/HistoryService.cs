using MysticWalley.Models;
using SQLite;

namespace MysticWalley.Services;

public class HistoryService
{
    private readonly SQLiteAsyncConnection _db;

    public HistoryService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "history.db3");
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<ChatMessage>().Wait(); // авто‑создание таблицы
    }

    /// <summary>
    /// Сохраняет сообщение
    /// </summary>
    public Task SaveMessageAsync(ChatMessage msg)
    {
        return _db.InsertAsync(msg);
    }

    /// <summary>
    /// Получает всю историю (сортируем новые записи сверху)
    /// </summary>
    public Task<List<ChatMessage>> GetHistoryAsync()
    {
        return _db.Table<ChatMessage>()
                  .OrderByDescending(m => m.Timestamp)
                  .ToListAsync();
    }
    public Task ClearHistoryAsync()
    {
        return _db.DeleteAllAsync<ChatMessage>();
    }

}