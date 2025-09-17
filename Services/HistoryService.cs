using MysticWalley.Models;

namespace MysticWalley.Services;

public class HistoryService
{
    private readonly List<ChatMessage> _messages = new();

    public Task SaveMessageAsync(ChatMessage msg)
    {
        _messages.Add(msg);
        return Task.CompletedTask;
    }

    public Task<List<ChatMessage>> GetHistoryAsync()
    {
        return Task.FromResult(_messages.ToList());
    }
}