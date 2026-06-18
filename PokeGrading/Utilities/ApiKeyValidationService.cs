using PokeGrading.Utilities;

namespace PokeGrading.Utilities
{
    public class ApiKeyValidationService
    {
        private readonly DatabaseService _databaseService;

        public ApiKeyValidationService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public dynamic ValidateApiKey(
            string apiKeyHash)
        {
            return _databaseService
                .QuerySingleOrDefault<dynamic>(
                @"
                SELECT
                    ak.api_key_id,
                    ak.client_id,
                    ak.status,
                    bc.active
                FROM API_KEYS ak
                INNER JOIN B2B_CLIENTS bc
                    ON ak.client_id = bc.client_id
                WHERE ak.api_key_hash = @ApiKeyHash
                ",
                new Dictionary<string, object>
                {
                    { "ApiKeyHash", apiKeyHash }
                });
        }
    }
}