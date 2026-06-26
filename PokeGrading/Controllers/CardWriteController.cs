using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Repositories;
using PokeGrading.Services;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardWriteController : ControllerBase
    {
        // Código de error SQL Server para violación de unique key
        private const int SqlUniqueKeyViolationError = 2627;

        private readonly ICardRepository _cardRepository;
        private readonly ICardValidationService _cardValidationService;

        public CardWriteController(
            ICardRepository cardRepository,
            ICardValidationService cardValidationService)
        {
            _cardRepository = cardRepository;
            _cardValidationService = cardValidationService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Data_output_add_card>> CreateCard(
            [FromForm] Data_input_add_card input)
        {
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

            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            string frontFileName = $"{Guid.NewGuid()}" + Path.GetExtension(input.front_image.FileName);
            string frontPath = Path.Combine(imagesFolder, frontFileName);

            using (var stream = new FileStream(frontPath, FileMode.Create))
            {
                await input.front_image.CopyToAsync(stream);
            }

            string frontUrl = $"/images/{frontFileName}";

            string? backUrl = null;

            if (input.back_image != null)
            {
                string backFileName = $"{Guid.NewGuid()}" + Path.GetExtension(input.back_image.FileName);
                string backPath = Path.Combine(imagesFolder, backFileName);

                using (var stream = new FileStream(backPath, FileMode.Create))
                {
                    await input.back_image.CopyToAsync(stream);
                }

                backUrl = $"/images/{backFileName}";
            }

            Guid cardId = Guid.NewGuid();
            Guid versionId = Guid.NewGuid();
            _cardRepository.InsertCard(cardId, input.created_by);

            // Programación defensiva: captura violación de unique key para el caso
            // de condición de carrera donde dos requests pasan el check de duplicado
            // al mismo tiempo e intentan insertar la misma carta concurrentemente
            try
            {
                _cardRepository.InsertCardVersion(versionId, cardId, input);
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueKeyViolationError)
            {
                return Conflict("Card already exists");
            }

            _cardRepository.UpdateCurrentVersion(cardId, versionId);
            _cardRepository.InsertCardImages(versionId, frontUrl, backUrl);
            _cardRepository.InsertAuditLog(
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
        public IActionResult CreateVersion([FromBody] Data_input_create_card_version input)
        {
            if (!_cardRepository.CardExists(input.card_id))
            {
                return NotFound("Card not found");
            }

            Guid versionId = Guid.NewGuid();

            _cardRepository.InsertCardVersion(versionId, input);
            _cardRepository.CopyImagesFromCurrentVersion(versionId, input.card_id);
            _cardRepository.UpdateCurrentVersion(input.card_id, versionId);
            _cardRepository.InsertAuditLog(
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