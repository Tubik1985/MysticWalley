using MysticWalley.Models;

namespace MysticWalley.Services;

// Сервис персонажей (пока всё статично, хардкод)
public static class CharacterService
{
    // Вернёт список доступных героев
    public static List<Character> GetCharacters() => new()
    {
        new Character { Name = "Астролог", Icon = "astrologer.png" },
        new Character { Name = "Гадалка",  Icon = "fortune.png" },
        new Character { Name = "Картоман", Icon = "cardman.png" }
    };
}