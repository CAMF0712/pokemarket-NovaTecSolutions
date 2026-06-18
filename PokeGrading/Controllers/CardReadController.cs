using Microsoft.AspNetCore.Mvc;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardReadController : ControllerBase
    {
        private const int ActiveCardFlag = 1;

        private readonly DatabaseService _database;

        public CardReadController(DatabaseService database)
        {
            _database = database;
        }

        [HttpGet("catalog")]
        public IActionResult GetCatalog()
        {
            var cards = _database.Query(
                @"
        SELECT
            c.card_id,
            cv.version_id,

            cv.name AS card_name,
            cv.set_name,
            cv.card_number,

            cv.edition,
            cv.language,
            cv.finish_type,

            cv.rarity,
            cv.pokemon_type,
            cv.hp,

            cv.illustrator,
            cv.release_year,

            ci.image_url

        FROM CARDS c

        INNER JOIN CARD_VERSIONS cv
            ON c.current_version_id =
               cv.version_id

        LEFT JOIN CARD_IMAGES ci
            ON cv.version_id =
               ci.version_id
            AND ci.image_type = 'FRONT'

        WHERE c.active = @active

        ORDER BY cv.name
        ",
                new()
                {
                    {"active", ActiveCardFlag}
                });

            return Ok(cards);
        }

        [HttpGet("{cardId}")]
        public IActionResult GetCard(Guid cardId)
        {
            var card = _database.QuerySingleOrDefault<dynamic>(
                @"
            SELECT
                c.card_id,
                cv.*
            FROM CARDS c
            INNER JOIN CARD_VERSIONS cv
                ON c.current_version_id =
                   cv.version_id
            WHERE c.card_id = @card_id
            ",
                new()
                {
                    {"card_id", cardId}
                });

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpGet("{cardId}/versions")]
        public IActionResult GetVersions(Guid cardId)
        {
            var versions = _database.Query(
                @"
            SELECT *
            FROM CARD_VERSIONS
            WHERE card_id = @card_id
            ORDER BY created_at DESC
            ",
                new()
                {
                    {"card_id", cardId}
                });

            return Ok(versions);
        }
    }
}