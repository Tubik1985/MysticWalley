namespace MysticWalley.Models;

public class Character
{
    // Имя персонажа (текст)
    public string Name { get; set; } = string.Empty;

    // Имя файла-иконки (например "astrologer.png")
    public string Icon { get; set; } = string.Empty;
}