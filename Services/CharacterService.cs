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
            Description = "✨ Глядит в звёзды и видит скрытые пути судьбы.",
            Background = "astrologer_bg.png"
        },
        new Character {
            Name = "Гадалка",
            Icon = "fortune.png",
            Portrait = "fortune_full.png",
            Description = "☕ В гуще кофе рождаются её загадочные предсказания.",
            Background = "fortune_bg.png"
        },
        new Character {
            Name = "Картоман",
            Icon = "cardman.png",
            Portrait = "cardman_full.png",
            Description = "🃏 Читает судьбу между строк колоды карт.",
            Background = "cardman_bg.png"
        }
    };
}