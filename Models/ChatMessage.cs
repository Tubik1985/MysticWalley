using SQLite;

namespace MysticWalley.Models;

public class ChatMessage
{
    [PrimaryKey, AutoIncrement] // авто‑инкремент для айдишника в базе
    public int Id { get; set; }

    public string UserMessage { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Character { get; set; } = string.Empty;
}