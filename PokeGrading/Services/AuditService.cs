using PokeGrading.Utilities;

namespace PokeGrading.Services
{
    public class AuditService : IAuditService
    {
        private readonly DatabaseService _database;

        public AuditService(DatabaseService database)
        {
            _database = database;
        }

        public void LogCardAction(
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

        public void LogApiCatalogCoverage(
            Guid apiKeyId,
            Guid requestId,
            int cardsCount)
        {
            _database.ExecuteNonQuery(
                @"
                INSERT INTO API_AUDIT_LOGS
                (
                    audit_id,
                    api_key_id,
                    request_id,
                    cards_count,
                    created_at
                )
                VALUES
                (
                    @AuditId,
                    @ApiKeyId,
                    @RequestId,
                    @CardsCount,
                    GETUTCDATE()
                )
                ",
                new()
                {
                    {"AuditId", Guid.NewGuid()},
                    {"ApiKeyId", apiKeyId},
                    {"RequestId", requestId},
                    {"CardsCount", cardsCount}
                });
        }
    }
}