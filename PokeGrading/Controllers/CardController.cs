using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using SixLabors.ImageSharp;


namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardController : ControllerBase
    {
        private readonly DatabaseService _database;

        public CardController(DatabaseService database)
        {
            _database = database;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Data_output_add_card>>
            CreateCard(
                [FromForm]
                Data_input_add_card input)
        {
            //----------------------------------
            // Required validations
            //----------------------------------

            if (string.IsNullOrWhiteSpace(input.card_name))
                return BadRequest("Card name required");

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

            //----------------------------------
            // Front image required
            //----------------------------------

            if (input.front_image == null)
            {
                return BadRequest(
                    "Front image required"
                );
            }

            //----------------------------------
            // Resolution validation
            //----------------------------------

            using (var image =
                SixLabors.ImageSharp.Image.Load(
                    input.front_image.OpenReadStream()))
            {
                if (image.Width < 600 ||
                    image.Height < 600)
                {
                    return BadRequest(
                        "Image resolution too low"
                    );
                }
            }


            //----------------------------------
            // Pokemon Type validation
            //----------------------------------

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
                return BadRequest(
                    "Invalid pokemon type"
                );
            }

            //----------------------------------
            // Rarity validation
            //----------------------------------

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
                return BadRequest(
                    "Invalid rarity"
                );
            }

            //----------------------------------
            // Language validation
            //----------------------------------

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
                return BadRequest(
                    "Invalid language"
                );
            }

            //----------------------------------
            // HP validation
            //----------------------------------

            if (input.hp <= 0)
            {
                return BadRequest(
                    "HP must be greater than zero"
                );
            }


            //----------------------------------
            // Duplicate validation
            //----------------------------------

            var existing =
                _database.QuerySingleOrDefault<Guid?>
                (
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
                        {"set_name",input.set_name},
                        {"card_number",input.card_number},
                        {"edition",input.edition},
                        {"language",input.language},
                        {"finish_type",input.finish_type}
                    }
                );

            if (existing != null)
            {
                return BadRequest(
                    "Card already exists");
            }

            //----------------------------------
            // Create Images folder
            //----------------------------------

            string imagesFolder =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Images");

            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(
                    imagesFolder);
            }

            //----------------------------------
            // Save Front Image
            //----------------------------------

            string frontFileName =
                $"{Guid.NewGuid()}" +
                Path.GetExtension(
                    input.front_image.FileName);

            string frontPath =
                Path.Combine(
                    imagesFolder,
                    frontFileName);

            using (var stream =
                new FileStream(
                    frontPath,
                    FileMode.Create))
            {
                await input.front_image
                    .CopyToAsync(stream);
            }

            string frontUrl =
                $"/images/{frontFileName}";

            //----------------------------------
            // Save Back Image (optional)
            //----------------------------------

            string? backUrl = null;

            if (input.back_image != null)
            {
                string backFileName =
                    $"{Guid.NewGuid()}" +
                    Path.GetExtension(
                        input.back_image.FileName);

                string backPath =
                    Path.Combine(
                        imagesFolder,
                        backFileName);

                using (var stream =
                    new FileStream(
                        backPath,
                        FileMode.Create))
                {
                    await input.back_image
                        .CopyToAsync(stream);
                }

                backUrl =
                    $"/images/{backFileName}";
            }

            //----------------------------------
            // IDs
            //----------------------------------

            Guid cardId = Guid.NewGuid();

            Guid versionId = Guid.NewGuid();

            Guid frontImageId = Guid.NewGuid();

            //----------------------------------
            // Insert Card
            //----------------------------------

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
                1,
                GETUTCDATE()
            )
            ",
            new()
            {
                {"card_id",cardId},
                {"created_by",input.created_by}
            });

            //----------------------------------
            // Insert Version
            //----------------------------------

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
                {"version_id",versionId},
                {"card_id",cardId},
                {"set_name",input.set_name},
                {"card_number",input.card_number},
                {"edition",input.edition},
                {"language",input.language},
                {"finish_type",input.finish_type},
                {"name",input.card_name},
                {"rarity",input.rarity},
                {"pokemon_type",input.pokemon_type},
                {"hp",input.hp},
                {"illustrator",input.illustrator},
                {"release_year",input.release_year},
                {"created_by",input.created_by}
            });

            //----------------------------------
            // Update version
            //----------------------------------

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

            //----------------------------------
            // Front Image
            //----------------------------------

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
                {"image_id",frontImageId},
                {"version_id",versionId},
                {"image_url",frontUrl}
            });

            //----------------------------------
            // Back Image
            //----------------------------------

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
                    {"image_id",Guid.NewGuid()},
                    {"version_id",versionId},
                    {"image_url",backUrl}
                });
            }

            //----------------------------------
            // Audit Log
            //----------------------------------

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

            //----------------------------------
            // Response
            //----------------------------------

            return Ok(
                new Data_output_add_card
                {
                    card_id = cardId,
                    version_id = versionId,
                    card_name = input.card_name
                });
        }

        [HttpPost("version")]
        public IActionResult CreateVersion(
        [FromBody]
            Data_input_create_card_version input)
                {
                    //----------------------------------
                    // Verify Card Exists
                    //----------------------------------

                    var cardExists =
                        _database.QuerySingleOrDefault<Guid?>
                        (
                            @"
                    SELECT card_id
                    FROM CARDS
                    WHERE card_id = @card_id
                    ",
                            new()
                            {
                        {"card_id", input.card_id}
                            }
                        );

                    if (cardExists == null)
                    {
                        return NotFound(
                            "Card not found"
                        );
                    }

                    //----------------------------------
                    // Create New Version
                    //----------------------------------

                    Guid versionId =
                        Guid.NewGuid();

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

                    //----------------------------------
                    // Update Current Version
                    //----------------------------------

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

                    //----------------------------------
                    // Audit
                    //----------------------------------

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

        [HttpGet("catalog")]
        public IActionResult GetCatalog()
        {
                var cards =
                    _database.Query(
                    @"
            SELECT
                c.card_id,
                cv.version_id,
                cv.name AS card_name,
                cv.set_name,
                cv.card_number,
                cv.rarity,
                cv.pokemon_type,
                cv.hp,
                ci.image_url
            FROM CARDS c
            INNER JOIN CARD_VERSIONS cv
                ON c.current_version_id =
                   cv.version_id
            LEFT JOIN CARD_IMAGES ci
                ON cv.version_id =
                   ci.version_id
                AND ci.image_type = 'FRONT'
            WHERE c.active = 1
            ORDER BY cv.name
            ",
                    new()
                    );

                return Ok(cards);
        }

    }
}