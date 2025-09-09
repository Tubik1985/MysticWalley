namespace MysticWalley.Models;

public class Character
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;        // маленькая иконка
    public string Portrait { get; set; } = string.Empty;    // крупная картинка для карточки
    public string Description { get; set; } = string.Empty;
}