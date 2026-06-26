using System.Text.RegularExpressions;

namespace PokeGrading.Services
{
    public interface IOcrParsingService
    {
        CardAttributesFromOcr ParseCardAttributes(string extractedText);
    }

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
