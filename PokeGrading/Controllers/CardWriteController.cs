// Controlador HTTP: coordina el flujo de entrada/salida para CardWriteController.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Repositories;
using PokeGrading.Services;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    /// <summary>
    /// Clase principal que concentra la responsabilidad de CardWriteController en esta capa.
    /// </summary>
    public class CardWriteController : ControllerBase
    {
        private const int SqlUniqueKeyViolationError = 2627;

        private readonly ICardRepository _cardRepository;
        private readonly ICardValidationService _cardValidationService;
        private readonly IImageStorageService _imageStorageService;
        private readonly IAuditService _auditService;

        /// <summary>
        /// Inicializa una nueva instancia de CardWriteController.
        /// </summary>
        public CardWriteController(
            ICardRepository cardRepository,
            ICardValidationService cardValidationService,
            IImageStorageService imageStorageService,
            IAuditService auditService)
        {
            _cardRepository = cardRepository;
            _cardValidationService = cardValidationService;
            _imageStorageService = imageStorageService;
            _auditService = auditService;
        }

        [HttpPost("create")]
        /// <summary>
        /// Crea una nueva carta con su version inicial e imagenes asociadas.
        /// </summary>
        public async Task<ActionResult<Data_output_add_card>> CreateCard(
            [FromForm] Data_input_add_card input)
        {
            this.EnsureTraceId();

            string? validationError =
                _cardValidationService
                .ValidateCreateCardInput(input);

            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var existing = _cardRepository.FindExistingCardVersionId(
                input.set_name,
                input.card_number,
                input.edition,
                input.language,
                input.finish_type);

            if (existing != null)
            {
                return BadRequest("Card already exists");
            }

            var storedImages =
                await _imageStorageService
                    .SaveCardImagesAsync(
                        input.front_image,
                        input.back_image);

            Guid cardId = Guid.NewGuid();
            Guid versionId = Guid.NewGuid();
            _cardRepository.InsertCard(cardId, input.created_by);

            try
            {
                _cardRepository.InsertCardVersion(versionId, cardId, input);
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueKeyViolationError)
            {
                return Conflict("Card already exists");
            }

            _cardRepository.UpdateCurrentVersion(cardId, versionId);
            _cardRepository.InsertCardImages(
                versionId,
                storedImages.FrontImageUrl,
                storedImages.BackImageUrl);

            _auditService.LogCardAction(
                input.created_by,
                "CREATE_CARD",
                cardId,
                input.card_name);

            return Ok(new Data_output_add_card
            {
                card_id = cardId,
                version_id = versionId,
                card_name = input.card_name
            });
        }

        [HttpPost("version")]
        /// <summary>
        /// Crea o actualiza la version actual de una carta segun la identidad recibida.
        /// </summary>
        public IActionResult CreateVersion([FromBody] Data_input_create_card_version input)
        {
            this.EnsureTraceId();

            if (!_cardRepository.CardExists(input.card_id))
            {
                return NotFound("Card not found");
            }

            var existingCardId = _cardRepository.FindExistingCardVersionId(
                input.set_name,
                input.card_number,
                input.edition,
                input.language,
                input.finish_type);

            if (existingCardId != null && existingCardId != input.card_id)
            {
                return Conflict("Card version already exists");
            }

            if (existingCardId == input.card_id)
            {
                var currentVersionId = _cardRepository.GetCurrentVersionId(input.card_id);

                if (currentVersionId == null)
                {
                    return NotFound("Card current version not found");
                }

                _cardRepository.UpdateCardVersion(currentVersionId.Value, input);

                _auditService.LogCardAction(
                    input.created_by,
                    "UPDATE_CARD",
                    input.card_id,
                    input.card_name);

                return Ok(new
                {
                    status = true,
                    version_id = currentVersionId.Value
                });
            }

            Guid versionId = Guid.NewGuid();

            try
            {
                _cardRepository.InsertCardVersion(versionId, input);
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueKeyViolationError)
            {
                return Conflict("Card version already exists");
            }

            _cardRepository.CopyImagesFromCurrentVersion(versionId, input.card_id);
            _cardRepository.UpdateCurrentVersion(input.card_id, versionId);
            _auditService.LogCardAction(
                input.created_by,
                "UPDATE_CARD",
                input.card_id,
                input.card_name);

            return Ok(new
            {
                status = true,
                version_id = versionId
            });
        }
    }
}

