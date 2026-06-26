using PokeGrading.Data_input_models;

namespace PokeGrading.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<dynamic> GetCatalog();

        dynamic? GetCard(Guid cardId);

        IEnumerable<dynamic> GetVersions(Guid cardId);

        Guid? FindExistingCardVersionId(
            string setName,
            string cardNumber,
            string edition,
            string language,
            string finishType);

        void InsertCard(Guid cardId, Guid createdBy);

        void InsertCardVersion(Guid versionId, Guid cardId, Data_input_add_card input);

        void InsertCardVersion(Guid versionId, Data_input_create_card_version input);

        void CopyImagesFromCurrentVersion(Guid newVersionId, Guid cardId);

        void UpdateCurrentVersion(Guid cardId, Guid versionId);

        void InsertCardImages(Guid versionId, string frontImageUrl, string? backImageUrl);

        void InsertAuditLog(Guid userId, string actionType, Guid entityId, string newValue);

        bool CardExists(Guid cardId);
    }
}