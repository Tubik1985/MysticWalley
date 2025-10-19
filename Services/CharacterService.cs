using MysticWalley.Models;
using System.Collections.Generic;
using System.Linq;

namespace MysticWalley.Services
{
    public class CharacterService
    {
        // Я вынес список в приватное поле, чтобы он не создавался заново при каждом вызове.
        // Это более эффективно.
        private readonly List<Character> _characters;

        public CharacterService()
        {
            _characters = new()
            {
                // === Мужчины ===
                new Character {
                    HeroId = "warlock",
                    Name = "Чернокнижник",
                    Icon = "warlock.png",
                    Portrait = "warlock_full.png",
                    Description = "😈 Повелитель Тёмной Башни, таинственная сила, шепчущая из тьмы.",
                    Background = "warlock_bg.png"
                },
                new Character {
                    HeroId = "astrologer",
                    Name = "Астролог Ориэн",
                    Icon = "astrologer.png",
                    Portrait = "astrologer_full.png",
                    Description = "🌌 Холодный исследователь небес, потерявший покой между формулами и чувствами.",
                    Background = "astrologer_bg.png"
                },
                new Character {
                    HeroId = "numerologist",
                    Name = "Нумеролог Исаак",
                    Icon = "numerologist.png",
                    Portrait = "numerologist_full.png",
                    Description = "🔢 Видит судьбу как уравнение, где даже ошибки имеют смысл.",
                    Background = "numerologist_bg.png"
                },
                new Character {
                    HeroId = "cardmaster",
                    Name = "Картомант Рамиэль",
                    Icon = "cardman.png",
                    Portrait = "cardman_full.png",
                    Description = "🃏 Фокусник и шут, превращающий случай в откровение.",
                    Background = "cardman_bg.png"
                },
                new Character {
                    HeroId = "shaman",
                    Name = "Шаман Мокелл",
                    Icon = "shaman.png",
                    Portrait = "shaman_full.png",
                    Description = "🪘 Слышит голоса духов и смеётся с ними — хаос его искусство.",
                    Background = "shaman_bg.png"
                },

                // === Женщины ===
                new Character {
                    HeroId = "matchmaker",
                    Name = "Сводница Лисса",
                    Icon = "matchmaker.png",
                    Portrait = "matchmaker_full.png",
                    Description = "💘 Лёгкая и остроумная флиртунья, в чьих словах всегда больше смысла, чем кажется.",
                    Background = "matchmaker_bg.png"
                },
                new Character {
                    HeroId = "fatekeeper",
                    Name = "Хранительница Селена",
                    Icon = "fatekeeper.png",
                    Portrait = "fatekeeper_full.png",
                    Description = "📖 Ведёт Книгу Судеб, где сама реальность пишет новые строки.",
                    Background = "fatekeeper_bg.png"
                },
                new Character {
                    HeroId = "flamepriestess",
                    Name = "Жрица Пламени Сарра",
                    Icon = "flamepriestess.png",
                    Portrait = "flamepriestess_full.png",
                    Description = "🔥 Учительница силы и мужества, превращает сомнения в огонь.",
                    Background = "flamepriestess_bg.png"
                },
                new Character {
                    HeroId = "dreamseer",
                    Name = "Толковательница снов Лира",
                    Icon = "dreamseer.png",
                    Portrait = "dreamseer_full.png",
                    Description = "😴 Между снами и реальностью, слышит шёпот Башни во сне.",
                    Background = "dreamseer_bg.png"
                }
            };
        }

        /// <summary>
        /// Возвращает полный список всех персонажей.
        /// </summary>
        public List<Character> GetCharacters() => _characters;


        // =========================================================================
        // ВОТ ЧТО Я ДОБАВИЛ: Метод для поиска персонажа по его ID.
        // =========================================================================
        /// <summary>
        /// Находит и возвращает одного персонажа по его уникальному ID.
        /// </summary>
        /// <param name="id">ID персонажа (например, "warlock").</param>
        /// <returns>Объект Character или null, если персонаж не найден.</returns>
        public Character? GetCharacterById(string id)
        {
            // Используем LINQ для поиска первого совпадения в нашем списке.
            // StringComparison.OrdinalIgnoreCase делает поиск нечувствительным к регистру.
            return _characters.FirstOrDefault(c => c.HeroId.Equals(id, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}