namespace PokeGrading.Services
{
    public interface IAuditService
    {
        void LogCardAction(
            Guid userId,
            string actionType,
            Guid entityId,
            string newValue);

        void LogApiCatalogCoverage(
            Guid apiKeyId,
            Guid requestId,
            int cardsCount);
    }
}