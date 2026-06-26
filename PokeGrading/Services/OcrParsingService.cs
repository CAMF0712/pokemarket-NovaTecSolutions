// Servicio: concentra logica de negocio y soporte para OcrParsingService.
using System.Text.RegularExpressions;

namespace PokeGrading.Services
{
    /// <summary>
    /// Contrato para transformar texto OCR en atributos estructurados.
    /// </summary>
    public interface IOcrParsingService
    {
        /// <summary>
        /// Extrae atributos clave (HP, numero y tipo) desde el texto reconocido.
        /// </summary>
        CardAttributesFromOcr ParseCardAttributes(string extractedText);
    }

    /// <summary>
    /// Implementa reglas de parseo para convertir texto OCR en datos de busqueda.
    /// </summary>
    public class OcrParsingService : IOcrParsingService
    {
        private static readonly HashSet<string> ValidTypes =
            new(StringComparer.OrdinalIgnoreCase)
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

        /// <summary>
        /// Interpreta el texto OCR y extrae atributos estructurados de carta.
        /// </summary>
        public CardAttributesFromOcr ParseCardAttributes(string extractedText)
        {
            var result = new CardAttributesFromOcr();

            // Extract HP
            result.DetectedHp = ExtractHp(extractedText);

            // Extract card number
            result.DetectedCardNumber = ExtractCardNumber(extractedText);

            // Extract type
            result.DetectedType = ExtractType(extractedText);

            return result;
        }

        // Busca patrones numericos que representan HP.
        private int? ExtractHp(string text)
        {
            var hpMatch = Regex.Match(
                text,
                @"(\d+)\s*HP|HP\s*(\d+)",
                RegexOptions.IgnoreCase);

            if (hpMatch.Success)
            {
                string hpValue =
                    !string.IsNullOrWhiteSpace(hpMatch.Groups[1].Value)
                        ? hpMatch.Groups[1].Value
                        : hpMatch.Groups[2].Value;

                if (int.TryParse(hpValue, out int parsedHp))
                {
                    return parsedHp;
                }
            }

            return null;
        }

        // Detecta el numero de carta en formato x/y y devuelve x.
        private string? ExtractCardNumber(string text)
        {
            var numberMatch = Regex.Match(
                text,
                @"(\d{1,3})\s*/\s*(\d{1,3})");

            if (numberMatch.Success)
            {
                return numberMatch.Groups[1].Value;
            }

            return null;
        }

        // Localiza el tipo Pokemon en base a un catalogo controlado de tipos validos.
        private string? ExtractType(string text)
        {
            foreach (var type in ValidTypes)
            {
                if (text.Contains(type, StringComparison.OrdinalIgnoreCase))
                {
                    return type;
                }
            }

            return null;
        }
    }
}

