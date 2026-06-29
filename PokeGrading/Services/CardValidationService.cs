// Servicio: concentra logica de negocio y soporte para CardValidationService.
using PokeGrading.Data_input_models;
using PokeGrading.Utilities;

namespace PokeGrading.Services
{
    /// <summary>
    /// Clase principal que concentra la responsabilidad de CardValidationService en esta capa.
    /// </summary>
    public class CardValidationService : ICardValidationService
    {
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

        /// <summary>
        /// Valida que los datos de alta de carta cumplan reglas mínimas requeridas.
        /// </summary>
        public string? ValidateCreateCardInput(
            Data_input_add_card input)
        {
            //----------------------------------------------------
            // Campos obligatorios
            //----------------------------------------------------

            if (string.IsNullOrWhiteSpace(input.card_name))
            {
                return "Card name required";
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

            //----------------------------------------------------
            // Longitudes
            //----------------------------------------------------

            if (input.card_name.Length > MaxCardNameLength)
            {
                return $"Card name must not exceed {MaxCardNameLength} characters";
            }

            if (input.set_name.Length > MaxSetNameLength)
            {
                return $"Set name must not exceed {MaxSetNameLength} characters";
            }

            if (!string.IsNullOrWhiteSpace(input.illustrator) &&
                input.illustrator.Length > MaxIllustratorLength)
            {
                return $"Illustrator must not exceed {MaxIllustratorLength} characters";
            }

            //----------------------------------------------------
            // Imagen
            //----------------------------------------------------

            if (input.front_image == null)
            {
                return "Front image required";
            }

            if (!ImageValidationService.IsValidImage(input.front_image))
            {
                return "Invalid front image";
            }

            if (input.back_image != null &&
                !ImageValidationService.IsValidImage(input.back_image))
            {
                return "Invalid back image";
            }

            //----------------------------------------------------
            // Catálogos
            //----------------------------------------------------

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

            //----------------------------------------------------
            // Valores numéricos
            //----------------------------------------------------

            if (input.hp <= MinimumHpValue)
            {
                return "HP must be greater than zero";
            }

            //----------------------------------------------------
            // Todo correcto
            //----------------------------------------------------

            return null;
        }
    }
}