using MysticWalley.Models;

namespace MysticWalley.Services
{
    public class CharacterService
    {
        public List<Character> GetCharacters() => new()
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
}