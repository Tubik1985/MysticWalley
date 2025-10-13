using System.Collections.Generic;

namespace MysticWalley.Models
{
    // Корневой объект JSON (сезон)
    public class StoryRoot
    {
        public string SeasonId { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }

        // Список эпизодов сезона
        public List<Episode> Episodes { get; set; }
    }

    // Один эпизод (строка истории)
    public class Episode
    {
        public string Id { get; set; }
        public string Title { get; set; }

        // Список сцен внутри эпизода
        public List<Scene> Scenes { get; set; }

        // Id следующего эпизода
        public string Next { get; set; }
    }

    // Одна реплика или блок текста от героя
    public class Scene
    {
        // Указываем, кто говорит, ключ HeroId из CharacterService
        public string HeroId { get; set; }

        // Эмоциональный тег (joy, fear, mystery и т. д.)
        public string Emotion { get; set; }

        // Текст предсказания или реплики
        public string Text { get; set; }
    }
}