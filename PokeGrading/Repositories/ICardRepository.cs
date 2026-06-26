// Contrato de repositorio: define operaciones de persistencia para ICardRepository.
using PokeGrading.Data_input_models;

namespace PokeGrading.Repositories
{
    /// <summary>
    /// Define el contrato de acceso a datos para cartas, versiones e imagenes.
    /// </summary>
    public interface ICardRepository
    {
        /// <summary>
        /// Obtiene el catalogo de cartas activas con su version vigente.
        /// </summary>
        IEnumerable<dynamic> GetCatalog();

        /// <summary>
        /// Obtiene una carta por su identificador con datos de su version actual.
        /// </summary>
        dynamic? GetCard(Guid cardId);

        /// <summary>
        /// Recupera todas las versiones historicas de una carta.
        /// </summary>
        IEnumerable<dynamic> GetVersions(Guid cardId);

        /// <summary>
        /// Recupera candidatos activos para busqueda por OCR.
        /// </summary>
        IEnumerable<dynamic> GetActiveSearchCandidates();

        /// <summary>
        /// Busca una carta existente por su identidad funcional (set, numero, edicion, idioma y acabado).
        /// </summary>
        Guid? FindExistingCardVersionId(
            string setName,
            string cardNumber,
            string edition,
            string language,
            string finishType);

        /// <summary>
        /// Obtiene la version actual asociada a una carta.
        /// </summary>
        Guid? GetCurrentVersionId(Guid cardId);

        /// <summary>
        /// Actualiza los datos editables de una version especifica.
        /// </summary>
        void UpdateCardVersion(Guid versionId, Data_input_create_card_version input);

        /// <summary>
        /// Crea el registro base de carta.
        /// </summary>
        void InsertCard(Guid cardId, Guid createdBy);

        /// <summary>
        /// Inserta una nueva version a partir del modelo de alta de carta.
        /// </summary>
        void InsertCardVersion(Guid versionId, Guid cardId, Data_input_add_card input);

        /// <summary>
        /// Inserta una nueva version a partir del modelo de creacion de version.
        /// </summary>
        void InsertCardVersion(Guid versionId, Data_input_create_card_version input);

        /// <summary>
        /// Copia las imagenes de la version actual hacia una version nueva.
        /// </summary>
        void CopyImagesFromCurrentVersion(Guid newVersionId, Guid cardId);

        /// <summary>
        /// Marca la version indicada como la version actual de la carta.
        /// </summary>
        void UpdateCurrentVersion(Guid cardId, Guid versionId);

        /// <summary>
        /// Inserta referencias de imagen frontal y trasera para una version.
        /// </summary>
        void InsertCardImages(Guid versionId, string frontImageUrl, string? backImageUrl);

        /// <summary>
        /// Valida si la carta existe y permanece activa.
        /// </summary>
        bool CardExists(Guid cardId);
    }
}

