using Microsoft.AspNetCore.Mvc;
using FuzzySharp;
using PokeGrading.Utilities;
using Tesseract;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardSearchController : ControllerBase
    {
        private const int ActiveCardFlag = 1;
        private const int NameScoreWeight = 3;
        private const int CardNumberExactMatchScore = 250;
        private const int HpExactMatchScore = 150;
        private const int TypeExactMatchScore = 100;
        private const int MinimumCandidateScore = 60;
        private const int MaximumCandidateMatches = 10;
        private const int CandidateScoreWindow = 30;
        private const int MinimumBestMatchScore = 200;
        private const int FullConfidencePercentage = 100;
        private const int ConfidenceRoundingDecimals = 2;

        private readonly DatabaseService _database;

        public CardSearchController(DatabaseService database)
        {
            _database = database;
        }

        /// <summary>
        /// Busca cartas en el catálogo usando OCR sobre la imagen enviada.
        /// Extrae texto, detecta HP, número y tipo, y calcula un score de similitud
        /// contra todas las cartas activas para devolver los mejores candidatos.
        /// </summary>
        [HttpPost("search-by-image")]
        public async Task<IActionResult> SearchByImage(IFormFile image)
        {
            if (image == null)
            {
                return BadRequest("Image required");
            }

            string tempFolder = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            // El archivo temporal se limpia en el finally
            // para garantizar que se borre incluso si ocurre una excepción
            string tempFile = Path.Combine(tempFolder, $"{Guid.NewGuid()}.jpg");
            string extractedText = "";

            try
            {
                using (var stream = new FileStream(tempFile, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Programación defensiva: si tessdata no existe o la imagen es inválida
                // se captura la excepción y se retorna un error controlado
                try
                {
                    using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
                    using var img = Pix.LoadFromFile(tempFile);
                    using var page = engine.Process(img);
                    extractedText = page.GetText();
                }
                catch (Exception)
                {
                    return BadRequest("Image could not be processed");
                }
            }
            finally
            {
                // Programación defensiva: garantiza limpieza del archivo temporal
                // independientemente del flujo de ejecución
                if (System.IO.File.Exists(tempFile))
                {
                    System.IO.File.Delete(tempFile);
                }
            }

            extractedText = extractedText.Replace("\r", " ").Replace("\n", " ").Trim();

            int? detectedHp = null;

            var hpMatch = System.Text.RegularExpressions.Regex.Match(
                extractedText,
                @"(\d+)\s*HP|HP\s*(\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            if (hpMatch.Success)
            {
                string hpValue =
                    !string.IsNullOrWhiteSpace(hpMatch.Groups[1].Value)
                        ? hpMatch.Groups[1].Value
                        : hpMatch.Groups[2].Value;

                // Programación defensiva: usar TryParse en lugar de Convert.ToInt32
                // para evitar excepción si el valor extraído por OCR no es parseable
                if (int.TryParse(hpValue, out int parsedHp))
                {
                    detectedHp = parsedHp;
                }
            }

            string? detectedNumber = null;

            var numberMatch = System.Text.RegularExpressions.Regex.Match(
                extractedText,
                @"(\d{1,3})\s*/\s*(\d{1,3})");

            if (numberMatch.Success)
            {
                detectedNumber = numberMatch.Groups[1].Value;
            }

            string[] types =
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

            string? detectedType = null;

            foreach (var type in types)
            {
                if (extractedText.Contains(type, StringComparison.OrdinalIgnoreCase))
                {
                    detectedType = type;
                    break;
                }
            }

            var candidates = _database.Query(
                @"
                SELECT
                    c.card_id,
                    cv.version_id,
                    cv.name AS card_name,
                    cv.card_number,
                    cv.hp,
                    cv.pokemon_type,
                    cv.set_name,
                    cv.rarity,
                    ci.image_url
                FROM CARDS c
                INNER JOIN CARD_VERSIONS cv
                    ON c.current_version_id =
                       cv.version_id
                LEFT JOIN CARD_IMAGES ci
                    ON cv.version_id =
                       ci.version_id
                   AND ci.image_type='FRONT'
                WHERE c.active = @active
                ",
                new()
                {
                    {"active", ActiveCardFlag}
                });

            var results = candidates
                .Select(card =>
                {
                    int score = 0;

                    string cardName = card.card_name?.ToString() ?? "";
                    string cardNumber = card.card_number?.ToString() ?? "";
                    string cardType = card.pokemon_type?.ToString() ?? "";
                    int cardHp = card.hp ?? 0;

                    int tokenScore = Fuzz.TokenSetRatio(extractedText, cardName);
                    int partialScore = Fuzz.PartialRatio(extractedText, cardName);
                    int nameScore = Math.Max(tokenScore, partialScore);

                    score += nameScore * NameScoreWeight;

                    string setName = card.set_name?.ToString() ?? "";
                    int setScore = Fuzz.PartialRatio(extractedText.ToLower(), setName.ToLower());

                    score += setScore;

                    if (!string.IsNullOrWhiteSpace(detectedNumber) && cardNumber == detectedNumber)
                    {
                        score += CardNumberExactMatchScore;
                    }

                    if (detectedHp != null && cardHp == detectedHp)
                    {
                        score += HpExactMatchScore;
                    }

                    if (detectedType != null && cardType.Equals(detectedType, StringComparison.OrdinalIgnoreCase))
                    {
                        score += TypeExactMatchScore;
                    }

                    Console.WriteLine($"{cardName} => {score}");

                    return new
                    {
                        score,
                        card
                    };
                })
                .Where(x => x.score >= MinimumCandidateScore)
                .OrderByDescending(x => x.score)
                .Take(MaximumCandidateMatches)
                .ToList();

            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }

            if (results.Count == 0)
            {
                return Ok(new
                {
                    found = false,
                    extracted_text = extractedText
                });
            }

            var bestMatch = results.First();
            const int scoreWindowThreshold = CandidateScoreWindow;

            var filteredMatches = results
                .Where(x => bestMatch.score - x.score <= scoreWindowThreshold)
                .ToList();

            if (bestMatch.score < MinimumBestMatchScore)
            {
                return Ok(new
                {
                    found = false,
                    reason = "LOW_CONFIDENCE",
                    extracted_text = extractedText
                });
            }

            double confidence;

            if (filteredMatches.Count == 1)
            {
                confidence = FullConfidencePercentage;
            }
            else
            {
                // Programación defensiva: verificar que existan al menos 2 candidatos
                // antes de acceder al segundo elemento para calcular la diferencia de score
                if (filteredMatches.Count >= 2)
                {
                    var secondBest = filteredMatches.Skip(1).First();

                    confidence = Math.Round(
                        ((double)(bestMatch.score - secondBest.score) / bestMatch.score) * FullConfidencePercentage,
                        ConfidenceRoundingDecimals);
                }
                else
                {
                    confidence = FullConfidencePercentage;
                }
            }

            return Ok(new
            {
                found = true,
                confidence,
                extracted_text = extractedText,
                detected_hp = detectedHp,
                detected_number = detectedNumber,
                detected_type = detectedType,
                best_match = bestMatch,
                candidate_matches = filteredMatches
            });
        }
    }
}