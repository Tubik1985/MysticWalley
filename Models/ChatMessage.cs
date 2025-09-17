namespace MysticWalley.Models;

public class ChatMessage
{
    public string UserMessage { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Character { get; set; } = string.Empty;
}