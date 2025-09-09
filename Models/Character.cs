namespace MysticWalley.Models;

public class Character
{
    public string Name { get; set; } = string.Empty;        // Имя
    public string Icon { get; set; } = string.Empty;        // Иконка (для списка на главной)
    public string Portrait { get; set; } = string.Empty;    // Портрет (для страницы героя)
    public string Description { get; set; } = string.Empty; // Краткое описание
    public string Background { get; set; } = string.Empty;  // Фон PredictionPage
}