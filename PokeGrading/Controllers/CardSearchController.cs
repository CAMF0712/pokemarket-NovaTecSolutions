using Microsoft.AspNetCore.Mvc;
using PokeGrading.Services;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardSearchController : ControllerBase
    {
        private const int ActiveCardFlag = 1;

        private readonly DatabaseService _database;
        private readonly IImageStorageService _imageStorageService;
        private readonly IOcrService _ocrService;
        private readonly IOcrParsingService _ocrParsingService;
        private readonly ISearchScoringService _searchScoringService;

        public CardSearchController(
            DatabaseService database,
            IImageStorageService imageStorageService,
            IOcrService ocrService,
            IOcrParsingService ocrParsingService,
            ISearchScoringService searchScoringService)
        {
            _database = database;
            _imageStorageService = imageStorageService;
            _ocrService = ocrService;
            _ocrParsingService = ocrParsingService;
            _searchScoringService = searchScoringService;
        }

        /// <summary>
        /// Busca cartas en el catálogo usando OCR sobre la imagen enviada.
        /// Orquesta OCRService para extracción de texto, OCRParsingService para
        /// interpretación de atributos y SearchScoringService para cálculo de similitud.
        /// </summary>
        [HttpPost("search-by-image")]
        public async Task<IActionResult> SearchByImage(IFormFile image)
        {
            if (image == null)
            {
                return BadRequest("Image required");
            }

            // Guardar imagen temporalmente
            var temporaryImage =
                await _imageStorageService
                    .SaveTemporarySearchImageAsync(image);

            string tempFile = temporaryImage.FilePath;
            string extractedText = "";

            try
            {
                // Extraer texto de la imagen usando OCR
                try
                {
                    extractedText =
                        await _ocrService
                            .ExtractTextAsync(tempFile);
                }
                catch (Exception)
                {
                    return BadRequest("Image could not be processed");
                }

                // Parsear atributos de la carta del texto extraído
                var cardAttributes =
                    _ocrParsingService
                        .ParseCardAttributes(extractedText);

                // Obtener catálogo de cartas activas
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

                // Calcular scores y confianza
                var searchResult =
                    _searchScoringService
                        .ScoreCandidates(
                            candidates,
                            extractedText,
                            cardAttributes);

                // Responder según el resultado
                if (searchResult.BestMatch == null)
                {
                    return Ok(new
                    {
                        found = false,
                        reason = "LOW_CONFIDENCE",
                        extracted_text = extractedText
                    });
                }

                return Ok(new
                {
                    found = true,
                    confidence = searchResult.Confidence,
                    extracted_text = extractedText,
                    detected_hp = cardAttributes.DetectedHp,
                    detected_number = cardAttributes.DetectedCardNumber,
                    detected_type = cardAttributes.DetectedType,
                    best_match = searchResult.BestMatch,
                    candidate_matches = searchResult.ScoredCandidates
                });
            }
            finally
            {
                // Limpiar archivo temporal
                _imageStorageService.DeleteFilesIfExist(tempFile);
            }
        }
    }
}