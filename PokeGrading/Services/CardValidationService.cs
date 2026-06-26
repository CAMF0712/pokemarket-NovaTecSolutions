using PokeGrading.Data_input_models;
using SixLabors.ImageSharp;

namespace PokeGrading.Services
{
    public class CardValidationService : ICardValidationService
    {
        private const int MinimumImageWidth = 600;
        private const int MinimumImageHeight = 600;
        private const int MinimumHpValue = 0;

        private const int MaxCardNameLength = 150;
        private const int MaxSetNameLength = 150;
        private const int MaxIllustratorLength = 100;

        private static readonly HashSet<string> ValidTypes =
            new(StringComparer.Ordinal)
            {
                "Grass",
                "Fire",
                "Water",
                "Lightning",
                "Psychic",
                "Fighting",
                "Darkness",
                "Metal",
                "Dragon",
                "Fairy",
                "Colorless"
            };

        private static readonly HashSet<string> ValidRarities =
            new(StringComparer.Ordinal)
            {
                "Common",
                "Uncommon",
                "Rare",
                "Holo Rare",
                "Ultra Rare",
                "Secret Rare"
            };

        private static readonly HashSet<string> ValidLanguages =
            new(StringComparer.Ordinal)
            {
                "English",
                "Spanish",
                "Japanese",
                "German",
                "French",
                "Italian",
                "Portuguese"
            };

        public string? ValidateCreateCardInput(Data_input_add_card input)
        {
            if (string.IsNullOrWhiteSpace(input.card_name))
            {
                return "Card name required";
            }

            if (input.card_name.Length > MaxCardNameLength)
            {
                return $"Card name must not exceed {MaxCardNameLength} characters";
            }

            if (!string.IsNullOrWhiteSpace(input.set_name) && input.set_name.Length > MaxSetNameLength)
            {
                return $"Set name must not exceed {MaxSetNameLength} characters";
            }

            if (!string.IsNullOrWhiteSpace(input.illustrator) && input.illustrator.Length > MaxIllustratorLength)
            {
                return $"Illustrator must not exceed {MaxIllustratorLength} characters";
            }

            if (string.IsNullOrWhiteSpace(input.set_name))
            {
                return "Set name required";
            }

            if (string.IsNullOrWhiteSpace(input.card_number))
            {
                return "Card number required";
            }

            if (string.IsNullOrWhiteSpace(input.edition))
            {
                return "Edition required";
            }

            if (string.IsNullOrWhiteSpace(input.language))
            {
                return "Language required";
            }

            if (string.IsNullOrWhiteSpace(input.finish_type))
            {
                return "Finish type required";
            }

            if (input.front_image == null)
            {
                return "Front image required";
            }

            try
            {
                using var image = Image.Load(input.front_image.OpenReadStream());

                if (image.Width < MinimumImageWidth || image.Height < MinimumImageHeight)
                {
                    return "Image resolution too low";
                }
            }
            catch (Exception)
            {
                return "Invalid image file";
            }

            if (!ValidTypes.Contains(input.pokemon_type))
            {
                return "Invalid pokemon type";
            }

            if (!ValidRarities.Contains(input.rarity))
            {
                return "Invalid rarity";
            }

            if (!ValidLanguages.Contains(input.language))
            {
                return "Invalid language";
            }

            if (input.hp <= MinimumHpValue)
            {
                return "HP must be greater than zero";
            }

            return null;
        }
    }
}