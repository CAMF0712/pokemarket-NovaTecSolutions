using Microsoft.AspNetCore.Mvc;
using PokeGrading.Repositories;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardReadController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;

        public CardReadController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [HttpGet("catalog")]
        public IActionResult GetCatalog()
        {
            var cards = _cardRepository.GetCatalog();

            return Ok(cards);
        }

        [HttpGet("{cardId}")]
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
        public IActionResult GetVersions(Guid cardId)
        {
            var versions = _cardRepository.GetVersions(cardId);

            return Ok(versions);
        }
    }
}