using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public CardController(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("add_card")]
        public ActionResult<
            Data_response<Data_output_add_card>>
            AddCard(
                Data_input_add_card input)
        {

            if (!ValidationService
                .ValidateCardFields(input))
            {
                return BadRequest(
                    "Required fields missing");
            }

            if (!ImageService
                .ValidateImage(input.image_url))
            {
                return BadRequest(
                    "Invalid image");
            }

            var duplicate =
                _databaseService
                .QuerySingleOrDefault<dynamic>(
                @"
                SELECT *
                FROM Cards
                WHERE
                SetName=@SetName
                AND CardNumber=@CardNumber
                AND Edition=@Edition
                AND Language=@Language
                AND FinishType=@FinishType
                ",
                new Dictionary<string, object>
                {
                    {"SetName", input.set_name},
                    {"CardNumber", input.card_number},
                    {"Edition", input.edition},
                    {"Language", input.language},
                    {"FinishType", input.finish_type}
                });

            if (duplicate != null)
            {
                return BadRequest(
                    "Card already exists");
            }

            Guid cardId =
                Guid.NewGuid();

            _databaseService.ExecuteNonQuery(
                @"
                INSERT INTO Cards
                (
                    CardId,
                    CreatedBy,

                    SetName,
                    CardNumber,
                    Edition,
                    Language,
                    FinishType,

                    Name,
                    Rarity,
                    PokemonType,
                    HP,
                    Illustrator,
                    ReleaseYear,

                    CreatedAt
                )
                VALUES
                (
                    @CardId,
                    @CreatedBy,

                    @SetName,
                    @CardNumber,
                    @Edition,
                    @Language,
                    @FinishType,

                    @Name,
                    @Rarity,
                    @PokemonType,
                    @HP,
                    @Illustrator,
                    @ReleaseYear,

                    GETDATE()
                )
                ",
                new Dictionary<string, object>
                {
                    {"CardId", cardId},
                    {"CreatedBy", input.created_by},

                    {"SetName", input.set_name},
                    {"CardNumber", input.card_number},
                    {"Edition", input.edition},
                    {"Language", input.language},
                    {"FinishType", input.finish_type},

                    {"Name", input.card_name},
                    {"Rarity", input.rarity},
                    {"PokemonType", input.pokemon_type},
                    {"HP", input.hp},
                    {"Illustrator", input.illustrator},
                    {"ReleaseYear", input.release_year}
                });

            return Ok(
                new Data_response<
                    Data_output_add_card>
                {
                    status = true,

                    data =
                    new Data_output_add_card
                    {
                        card_id = cardId,

                        card_name = input.card_name,

                        set_name = input.set_name,

                        card_number =
                            input.card_number,

                        created_at =
                            DateTime.UtcNow
                    }
                });
        }
    }
}