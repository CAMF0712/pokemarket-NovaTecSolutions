using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using SixLabors.ImageSharp;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("Card")]
    public class CardWriteController : ControllerBase
    {
        private const int MinimumImageWidth = 600;
        private const int MinimumImageHeight = 600;
        private const int MinimumHpValue = 0;
        private const int ActiveCardFlag = 1;

        // Programación defensiva: longitudes máximas alineadas con las columnas de la BD
        private const int MaxCardNameLength = 150;
        private const int MaxSetNameLength = 150;
        private const int MaxIllustratorLength = 100;

        // Código de error SQL Server para violación de unique key
        private const int SqlUniqueKeyViolationError = 2627;

        private readonly DatabaseService _database;

        public CardWriteController(DatabaseService database)
        {
            _database = database;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Data_output_add_card>> CreateCard(
            [FromForm] Data_input_add_card input)
        {
            if (string.IsNullOrWhiteSpace(input.card_name))
                return BadRequest("Card name required");

            // Programación defensiva: validar longitud de campos de texto
            // para evitar excepciones de BD por columnas con límite de caracteres
            if (input.card_name.Length > MaxCardNameLength)
                return BadRequest($"Card name must not exceed {MaxCardNameLength} characters");

            if (!string.IsNullOrWhiteSpace(input.set_name) && input.set_name.Length > MaxSetNameLength)
                return BadRequest($"Set name must not exceed {MaxSetNameLength} characters");

            if (!string.IsNullOrWhiteSpace(input.illustrator) && input.illustrator.Length > MaxIllustratorLength)
                return BadRequest($"Illustrator must not exceed {MaxIllustratorLength} characters");

            if (string.IsNullOrWhiteSpace(input.set_name))
                return BadRequest("Set name required");

            if (string.IsNullOrWhiteSpace(input.card_number))
                return BadRequest("Card number required");

            if (string.IsNullOrWhiteSpace(input.edition))
                return BadRequest("Edition required");

            if (string.IsNullOrWhiteSpace(input.language))
                return BadRequest("Language required");

            if (string.IsNullOrWhiteSpace(input.finish_type))
                return BadRequest("Finish type required");

            if (input.front_image == null)
            {
                return BadRequest("Front image required");
            }

            // Programación defensiva: Image.Load puede lanzar si el archivo está corrupto
            // o tiene un formato no soportado; se captura para devolver un error controlado
            try
            {
                using (var image = Image.Load(input.front_image.OpenReadStream()))
                {
                    if (image.Width < MinimumImageWidth || image.Height < MinimumImageHeight)
                    {
                        return BadRequest("Image resolution too low");
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest("Invalid image file");
            }

            string[] validTypes =
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

            if (!validTypes.Contains(input.pokemon_type))
            {
                return BadRequest("Invalid pokemon type");
            }

            string[] validRarities =
            {
                "Common",
                "Uncommon",
                "Rare",
                "Holo Rare",
                "Ultra Rare",
                "Secret Rare"
            };

            if (!validRarities.Contains(input.rarity))
            {
                return BadRequest("Invalid rarity");
            }

            string[] validLanguages =
            {
                "English",
                "Spanish",
                "Japanese",
                "German",
                "French",
                "Italian",
                "Portuguese"
            };

            if (!validLanguages.Contains(input.language))
            {
                return BadRequest("Invalid language");
            }

            if (input.hp <= MinimumHpValue)
            {
                return BadRequest("HP must be greater than zero");
            }

            var existing = _database.QuerySingleOrDefault<Guid?>(
                @"
                    SELECT TOP 1 card_id
                    FROM CARD_VERSIONS
                    WHERE
                        set_name=@set_name
                    AND card_number=@card_number
                    AND edition=@edition
                    AND language=@language
                    AND finish_type=@finish_type
                    ",
                new Dictionary<string, object>
                {
                    {"set_name", input.set_name},
                    {"card_number", input.card_number},
                    {"edition", input.edition},
                    {"language", input.language},
                    {"finish_type", input.finish_type}
                });

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
            Guid frontImageId = Guid.NewGuid();

            _database.ExecuteNonQuery(
                @"
            INSERT INTO CARDS
            (
                card_id,
                created_by,
                active,
                created_at
            )
            VALUES
            (
                @card_id,
                @created_by,
                @active,
                GETUTCDATE()
            )
            ",
                new()
                {
                    {"card_id", cardId},
                    {"created_by", input.created_by},
                    {"active", ActiveCardFlag}
                });

            // Programación defensiva: captura violación de unique key para el caso
            // de condición de carrera donde dos requests pasan el check de duplicado
            // al mismo tiempo e intentan insertar la misma carta concurrentemente
            try
            {
            _database.ExecuteNonQuery(
                @"
            INSERT INTO CARD_VERSIONS
            (
                version_id,
                card_id,
                set_name,
                card_number,
                edition,
                language,
                finish_type,
                name,
                rarity,
                pokemon_type,
                hp,
                illustrator,
                release_year,
                created_by,
                created_at
            )
            VALUES
            (
                @version_id,
                @card_id,
                @set_name,
                @card_number,
                @edition,
                @language,
                @finish_type,
                @name,
                @rarity,
                @pokemon_type,
                @hp,
                @illustrator,
                @release_year,
                @created_by,
                GETUTCDATE()
            )
            ",
                new()
                {
                    {"version_id", versionId},
                    {"card_id", cardId},
                    {"set_name", input.set_name},
                    {"card_number", input.card_number},
                    {"edition", input.edition},
                    {"language", input.language},
                    {"finish_type", input.finish_type},
                    {"name", input.card_name},
                    {"rarity", input.rarity},
                    {"pokemon_type", input.pokemon_type},
                    {"hp", input.hp},
                    {"illustrator", input.illustrator},
                    {"release_year", input.release_year},
                    {"created_by", input.created_by}
                });
            }
            catch (SqlException ex) when (ex.Number == SqlUniqueKeyViolationError)
            {
                return Conflict("Card already exists");
            }

            _database.ExecuteNonQuery(
                @"
            UPDATE CARDS
            SET current_version_id = @version_id
            WHERE card_id = @card_id
            ",
                new()
                {
                    {"version_id", versionId},
                    {"card_id", cardId}
                });

            _database.ExecuteNonQuery(
                @"
            INSERT INTO CARD_IMAGES
            (
                image_id,
                version_id,
                image_type,
                image_url
            )
            VALUES
            (
                @image_id,
                @version_id,
                'FRONT',
                @image_url
            )
            ",
                new()
                {
                    {"image_id", frontImageId},
                    {"version_id", versionId},
                    {"image_url", frontUrl}
                });

            if (backUrl != null)
            {
                _database.ExecuteNonQuery(
                    @"
                INSERT INTO CARD_IMAGES
                (
                    image_id,
                    version_id,
                    image_type,
                    image_url
                )
                VALUES
                (
                    @image_id,
                    @version_id,
                    'BACK',
                    @image_url
                )
                ",
                    new()
                    {
                        {"image_id", Guid.NewGuid()},
                        {"version_id", versionId},
                        {"image_url", backUrl}
                    });
            }

            _database.ExecuteNonQuery(
                @"
            INSERT INTO AUDIT_LOGS
            (
                audit_id,
                user_id,
                action_type,
                entity_name,
                entity_id,
                new_value,
                timestamp
            )
            VALUES
            (
                @audit_id,
                @user_id,
                'CREATE_CARD',
                'CARD',
                @entity_id,
                @new_value,
                GETUTCDATE()
            )
            ",
                new()
                {
                    {"audit_id", Guid.NewGuid()},
                    {"user_id", input.created_by},
                    {"entity_id", cardId},
                    {"new_value", input.card_name}
                });

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
            var cardExists = _database.QuerySingleOrDefault<Guid?>(
                @"
                    SELECT card_id
                    FROM CARDS
                    WHERE card_id = @card_id
                    ",
                new()
                {
                    {"card_id", input.card_id}
                });

            if (cardExists == null)
            {
                return NotFound("Card not found");
            }

            Guid versionId = Guid.NewGuid();

            _database.ExecuteNonQuery(
                @"
            INSERT INTO CARD_VERSIONS
            (
                version_id,
                card_id,
                set_name,
                card_number,
                edition,
                language,
                finish_type,
                name,
                rarity,
                pokemon_type,
                hp,
                illustrator,
                release_year,
                created_by,
                created_at
            )
            VALUES
            (
                @version_id,
                @card_id,
                @set_name,
                @card_number,
                @edition,
                @language,
                @finish_type,
                @name,
                @rarity,
                @pokemon_type,
                @hp,
                @illustrator,
                @release_year,
                @created_by,
                GETUTCDATE()
            )
            ",
                new()
                {
                    {"version_id", versionId},
                    {"card_id", input.card_id},
                    {"set_name", input.set_name},
                    {"card_number", input.card_number},
                    {"edition", input.edition},
                    {"language", input.language},
                    {"finish_type", input.finish_type},
                    {"name", input.card_name},
                    {"rarity", input.rarity},
                    {"pokemon_type", input.pokemon_type},
                    {"hp", input.hp},
                    {"illustrator", input.illustrator},
                    {"release_year", input.release_year},
                    {"created_by", input.created_by}
                });

            _database.ExecuteNonQuery(
                @"
            INSERT INTO CARD_IMAGES
            (
                image_id,
                version_id,
                image_type,
                image_url
            )
            SELECT
                NEWID(),
                @new_version_id,
                image_type,
                image_url
            FROM CARD_IMAGES
            WHERE version_id =
            (
                SELECT current_version_id
                FROM CARDS
                WHERE card_id = @card_id
            )
            ",
                new()
                {
                    {"new_version_id", versionId},
                    {"card_id", input.card_id}
                });

            _database.ExecuteNonQuery(
                @"
            UPDATE CARDS
            SET current_version_id = @version_id
            WHERE card_id = @card_id
            ",
                new()
                {
                    {"version_id", versionId},
                    {"card_id", input.card_id}
                });

            _database.ExecuteNonQuery(
                @"
            INSERT INTO AUDIT_LOGS
            (
                audit_id,
                user_id,
                action_type,
                entity_name,
                entity_id,
                new_value,
                timestamp
            )
            VALUES
            (
                @audit_id,
                @user_id,
                'UPDATE_CARD',
                'CARD',
                @entity_id,
                @new_value,
                GETUTCDATE()
            )
            ",
                new()
                {
                    {"audit_id", Guid.NewGuid()},
                    {"user_id", input.created_by},
                    {"entity_id", input.card_id},
                    {"new_value", input.card_name}
                });

            return Ok(new
            {
                status = true,
                version_id = versionId
            });
        }
    }
}