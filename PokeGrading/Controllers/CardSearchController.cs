// Controlador HTTP: coordina el flujo de entrada/salida para CardSearchController.
using Microsoft.AspNetCore.Mvc;
using PokeGrading.Repositories;
using PokeGrading.Services;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    /// <summary>
    /// Clase principal que concentra la responsabilidad de CardSearchController en esta capa.
    /// </summary>
    public class CardSearchController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IOcrService _ocrService;
        private readonly IOcrParsingService _ocrParsingService;
        private readonly ISearchScoringService _searchScoringService;

        /// <summary>
        /// Inicializa una nueva instancia de CardSearchController.
        /// </summary>
        public CardSearchController(
            ICardRepository cardRepository,
            IImageStorageService imageStorageService,
            IOcrService ocrService,
            IOcrParsingService ocrParsingService,
            ISearchScoringService searchScoringService)
        {
            _cardRepository = cardRepository;
            _imageStorageService = imageStorageService;
            _ocrService = ocrService;
            _ocrParsingService = ocrParsingService;
            _searchScoringService = searchScoringService;
        }

        [HttpPost("search-by-image")]
        /// <summary>
        /// Procesa una imagen, extrae texto por OCR y busca la mejor coincidencia en el catalogo.
        /// </summary>
        public async Task<IActionResult> SearchByImage(IFormFile image)
        {
            this.EnsureTraceId();

            if (image == null)
            {
                return BadRequest("Image required");
            }

            var temporaryImage =
                await _imageStorageService
                    .SaveTemporarySearchImageAsync(image);

            string tempFile = temporaryImage.FilePath;
            string extractedText = "";

            try
            {
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

                var cardAttributes =
                    _ocrParsingService
                        .ParseCardAttributes(extractedText);

                var candidates = _cardRepository.GetActiveSearchCandidates();

                var searchResult =
                    _searchScoringService
                        .ScoreCandidates(
                            candidates,
                            extractedText,
                            cardAttributes);

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
                _imageStorageService.DeleteFilesIfExist(tempFile);
            }
        }
    }
}


