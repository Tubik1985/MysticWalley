namespace MysticWalley.Models
{
    public class Character
    {
        // Уникальный ключ, связывает героя с его сценами в StoryConfig
        public string HeroId { get; set; }

        // Имя персонажа для отображения
        public string Name { get; set; }

        // Маленькая иконка (список выбора)
        public string Icon { get; set; }

        // Портрет для страницы предсказаний
        public string Portrait { get; set; }

        // Фон или сцена за персонажем (эстетика)
        public string Background { get; set; }

        // Краткое описание для окна выбора и информационных карточек
        public string Description { get; set; }

        // (Опционально) Параметры автоплейного “предсказательного” цвета или эмоции
        public string ThemeColor { get; set; }

        // (Опционально) Индикатор наличия пасхалки или истории
        public bool HasSecret { get; set; } = false;
    }
}