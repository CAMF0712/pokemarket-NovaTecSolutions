using PokeGrading.Data_input_models;

namespace PokeGrading.Repositories
{
    public interface ICardRepository
    {
        IEnumerable<dynamic> GetCatalog();

        dynamic? GetCard(Guid cardId);

        IEnumerable<dynamic> GetVersions(Guid cardId);

        IEnumerable<dynamic> GetActiveSearchCandidates();

        Guid? FindExistingCardVersionId(
            string setName,
            string cardNumber,
            string edition,
            string language,
            string finishType);

        Guid? GetCurrentVersionId(Guid cardId);

        void UpdateCardVersion(Guid versionId, Data_input_create_card_version input);

        void InsertCard(Guid cardId, Guid createdBy);

        void InsertCardVersion(Guid versionId, Guid cardId, Data_input_add_card input);

        void InsertCardVersion(Guid versionId, Data_input_create_card_version input);

        void CopyImagesFromCurrentVersion(Guid newVersionId, Guid cardId);

        void UpdateCurrentVersion(Guid cardId, Guid versionId);

        void InsertCardImages(Guid versionId, string frontImageUrl, string? backImageUrl);

        bool CardExists(Guid cardId);
    }
}
