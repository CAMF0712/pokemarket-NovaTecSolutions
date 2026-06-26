// Repositorio: encapsula acceso a datos para CardRepository.
using PokeGrading.Data_input_models;
using PokeGrading.Utilities;

namespace PokeGrading.Repositories
{
    /// <summary>
    /// Clase principal que concentra la responsabilidad de CardRepository en esta capa.
    /// </summary>
    public class CardRepository : ICardRepository
    {
        private const int ActiveCardFlag = 1;

        private readonly DatabaseService _database;

        /// <summary>
        /// Inicializa una nueva instancia de CardRepository.
        /// </summary>
        public CardRepository(DatabaseService database)
        {
            _database = database;
        }

        /// <summary>
        /// Obtiene el catalogo activo de cartas para exponerlo en la API.
        /// </summary>
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

        /// <summary>
        /// Obtiene el detalle de una carta por su identificador unico.
        /// </summary>
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

        /// <summary>
        /// Recupera el historial de versiones asociadas a una carta.
        /// </summary>
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

        /// <summary>
        /// Obtiene candidatos activos para el flujo de busqueda por imagen.
        /// </summary>
        public IEnumerable<dynamic> GetActiveSearchCandidates()
        {
            return _database.Query(
                @"
                SELECT
                    c.card_id,
                    cv.version_id,
                    cv.name AS card_name,
                    cv.card_number,
                    cv.hp,
                    cv.pokemon_type,
                    cv.set_name,
                    cv.rarity,
                    ci.image_url
                FROM CARDS c
                INNER JOIN CARD_VERSIONS cv
                    ON c.current_version_id =
                       cv.version_id
                LEFT JOIN CARD_IMAGES ci
                    ON cv.version_id =
                       ci.version_id
                   AND ci.image_type='FRONT'
                WHERE c.active = @active
                ",
                new()
                {
                    { "active", ActiveCardFlag }
                });
        }

        /// <summary>
        /// Busca una version existente con la misma identidad funcional de carta.
        /// </summary>
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


        /// <summary>
        /// Devuelve el identificador de la version actual registrada para una carta.
        /// </summary>
        public Guid? GetCurrentVersionId(Guid cardId)
        {
            return _database.QuerySingleOrDefault<Guid?>(
                @"
                SELECT current_version_id
                FROM CARDS
                WHERE card_id = @card_id
                ",
                new()
                {
                    {"card_id", cardId}
                });
        }

        /// <summary>
        /// Actualiza los campos editables de una version de carta existente.
        /// </summary>
        public void UpdateCardVersion(Guid versionId, Data_input_create_card_version input)
        {
            _database.ExecuteNonQuery(
                @"
                UPDATE CARD_VERSIONS
                SET
                    name = @name,
                    rarity = @rarity,
                    pokemon_type = @pokemon_type,
                    hp = @hp,
                    illustrator = @illustrator,
                    release_year = @release_year,
                    created_by = @created_by
                WHERE version_id = @version_id
                ",
                new()
                {
                    {"version_id", versionId},
                    {"name", input.card_name},
                    {"rarity", input.rarity},
                    {"pokemon_type", input.pokemon_type},
                    {"hp", input.hp},
                    {"illustrator", input.illustrator},
                    {"release_year", input.release_year},
                    {"created_by", input.created_by}
                });
        }
        /// <summary>
        /// Inserta el registro base de la carta en la tabla principal.
        /// </summary>
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

        /// <summary>
        /// Inserta una nueva version de carta con sus atributos de negocio.
        /// </summary>
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

        /// <summary>
        /// Inserta una nueva version de carta con sus atributos de negocio.
        /// </summary>
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

        /// <summary>
        /// Duplica referencias de imagen de la version actual hacia una nueva version.
        /// </summary>
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

        /// <summary>
        /// Marca que una version especifica pasa a ser la version actual de la carta.
        /// </summary>
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

        /// <summary>
        /// Registra URLs de imagenes asociadas a una version de carta.
        /// </summary>
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

        /// <summary>
        /// Verifica si una carta existe en base de datos y se encuentra activa.
        /// </summary>
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



