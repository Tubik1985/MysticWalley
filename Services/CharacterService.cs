using MysticWalley.Models;

namespace MysticWalley.Services;

public static class CharacterService
{
    public static List<Character> GetCharacters() => new()
    {
        new Character {
            Name = "Астролог",
            Icon = "astrologer.png",
            Portrait = "astrologer_full.png",
            Description = "✨ Глядит в звёзды и видит скрытые пути судьбы."
        },
        new Character {
            Name = "Гадалка",
            Icon = "fortune.png",
            Portrait = "fortune_full.png",
            Description = "☕ В гуще кофе рождаются её загадочные предсказания."
        },
        new Character {
            Name = "Картоман",
            Icon = "cardman.png",
            Portrait = "cardman_full.png",
            Description = "🃏 Читает судьбу между строк колоды карт."
        }
    };
}