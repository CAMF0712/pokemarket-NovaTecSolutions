using PokeGrading.Data_input_models;
using PokeGrading.Utilities;

namespace PokeGrading.Repositories
{
    public class CardRepository : ICardRepository
    {
        private const int ActiveCardFlag = 1;

        private readonly DatabaseService _database;

        public CardRepository(DatabaseService database)
        {
            _database = database;
        }

        public IEnumerable<dynamic> GetCatalog()
        {
            return _database.Query(
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
        }

        public dynamic? GetCard(Guid cardId)
        {
            return _database.QuerySingleOrDefault<dynamic>(
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
        }

        public IEnumerable<dynamic> GetVersions(Guid cardId)
        {
            return _database.Query(
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
        }

        public Guid? FindExistingCardVersionId(
            string setName,
            string cardNumber,
            string edition,
            string language,
            string finishType)
        {
            return _database.QuerySingleOrDefault<Guid?>(
                @"
                SELECT TOP 1 card_id
                FROM CARD_VERSIONS
                WHERE
                    set_name = @set_name
                AND card_number = @card_number
                AND edition = @edition
                AND language = @language
                AND finish_type = @finish_type
                ",
                new()
                {
                    {"set_name", setName},
                    {"card_number", cardNumber},
                    {"edition", edition},
                    {"language", language},
                    {"finish_type", finishType}
                });
        }

        public void InsertCard(Guid cardId, Guid createdBy)
        {
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
                    {"created_by", createdBy},
                    {"active", ActiveCardFlag}
                });
        }

        public void InsertCardVersion(
            Guid versionId,
            Guid cardId,
            Data_input_add_card input)
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

        public void InsertCardVersion(
            Guid versionId,
            Data_input_create_card_version input)
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
        }

        public void CopyImagesFromCurrentVersion(Guid newVersionId, Guid cardId)
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
                    {"new_version_id", newVersionId},
                    {"card_id", cardId}
                });
        }

        public void UpdateCurrentVersion(Guid cardId, Guid versionId)
        {
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
        }

        public void InsertCardImages(
            Guid versionId,
            string frontImageUrl,
            string? backImageUrl)
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
                    'FRONT',
                    @image_url
                )
                ",
                new()
                {
                    {"image_id", Guid.NewGuid()},
                    {"version_id", versionId},
                    {"image_url", frontImageUrl}
                });

            if (backImageUrl != null)
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
                        {"image_url", backImageUrl}
                    });
            }
        }

        public void InsertAuditLog(
            Guid userId,
            string actionType,
            Guid entityId,
            string newValue)
        {
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
                    @action_type,
                    'CARD',
                    @entity_id,
                    @new_value,
                    GETUTCDATE()
                )
                ",
                new()
                {
                    {"audit_id", Guid.NewGuid()},
                    {"user_id", userId},
                    {"action_type", actionType},
                    {"entity_id", entityId},
                    {"new_value", newValue}
                });
        }

        public bool CardExists(Guid cardId)
        {
            var existing = _database.QuerySingleOrDefault<Guid?>(
                @"
                SELECT card_id
                FROM CARDS
                WHERE card_id = @card_id
                ",
                new()
                {
                    {"card_id", cardId}
                });

            return existing != null;
        }
    }
}