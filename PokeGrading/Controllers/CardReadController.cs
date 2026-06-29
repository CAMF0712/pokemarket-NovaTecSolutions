// Controlador HTTP: coordina el flujo de entrada/salida para CardReadController.
using Microsoft.AspNetCore.Mvc;
using PokeGrading.Repositories;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    /// <summary>
    /// Clase principal que concentra la responsabilidad de CardReadController en esta capa.
    /// </summary>
    public class CardReadController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;

        /// <summary>
        /// Inicializa una nueva instancia de CardReadController.
        /// </summary>
        public CardReadController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpGet("catalog")]
        /// <summary>
        /// Obtiene el catalogo activo de cartas para exponerlo en la API.
        /// </summary>
        public IActionResult GetCatalog()
        {
            var cards = _cardRepository.GetCatalog();

            return Ok(cards);
        }

        [HttpGet("{cardId}")]
        /// <summary>
        /// Obtiene el detalle de una carta por su identificador unico.
        /// </summary>
        public IActionResult GetCard(Guid cardId)
        {
            var card = _cardRepository.GetCard(cardId);

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpGet("{cardId}/versions")]
        /// <summary>
        /// Recupera el historial de versiones asociadas a una carta.
        /// </summary>
        public IActionResult GetVersions(Guid cardId)
        {
            var versions = _cardRepository.GetVersions(cardId);

            return Ok(versions);
        }
    }
}
