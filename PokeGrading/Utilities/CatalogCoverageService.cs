using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;

namespace PokeGrading.Utilities
{
    public class CatalogCoverageService
    {
        private readonly DatabaseService _databaseService;

        public CatalogCoverageService(
            DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Data_output_catalog_result>
            ProcessCards(
                List<Data_input_catalog_lookup>
                cards)
        {
            var results =
                new List<
                    Data_output_catalog_result>();

            foreach (var card in cards)
            {
                results.Add(
                    ProcessSingleCard(card));
            }

            return results;
        }

        private Data_output_catalog_result
            ProcessSingleCard(
                Data_input_catalog_lookup card)
        {
            //-----------------------------------
            // Required fields
            //-----------------------------------

            if (
                string.IsNullOrWhiteSpace(
                    card.set_name)
                ||
                string.IsNullOrWhiteSpace(
                    card.card_number)
            )
            {
                return
                    new Data_output_catalog_result
                    {
                        status =
                            "INVALID_PARAMETERS",

                        message =
                            "set_name and card_number are required"
                    };
            }

            //-----------------------------------
            // Search candidates
            //-----------------------------------

            var candidates =
                _databaseService
                .QueryList<
                    Data_output_catalog_candidate>(
                @"
                SELECT
                    c.card_id,

                    cv.set_name,
                    cv.card_number,
                    cv.edition,
                    cv.language,
                    cv.finish_type

                FROM CARDS c

                INNER JOIN CARD_VERSIONS cv
                    ON c.current_version_id =
                       cv.version_id

                WHERE
                    c.active = 1

                    AND cv.set_name =
                        @SetName

                    AND cv.card_number =
                        @CardNumber

                    AND
                    (
                        @Edition IS NULL
                        OR
                        cv.edition =
                        @Edition
                    )

                    AND
                    (
                        @Language IS NULL
                        OR
                        cv.language =
                        @Language
                    )

                    AND
                    (
                        @FinishType IS NULL
                        OR
                        cv.finish_type =
                        @FinishType
                    )

                ORDER BY
                    cv.set_name,
                    cv.card_number,
                    cv.edition,
                    cv.language,
                    cv.finish_type
                ",
                new Dictionary<string, object>
                {
                    {
                        "SetName",
                        card.set_name
                    },

                    {
                        "CardNumber",
                        card.card_number
                    },

                    {
                        "Edition",
                        string.IsNullOrWhiteSpace(
                            card.edition)
                            ? null
                            : card.edition
                    },

                    {
                        "Language",
                        string.IsNullOrWhiteSpace(
                            card.language)
                            ? null
                            : card.language
                    },

                    {
                        "FinishType",
                        string.IsNullOrWhiteSpace(
                            card.finish_type)
                            ? null
                            : card.finish_type
                    }
                })
                .ToList();

            //-----------------------------------
            // No match
            //-----------------------------------

            if (candidates.Count == 0)
            {
                return
                    new Data_output_catalog_result
                    {
                        status = "NOT_COVERED",

                        message =
                            "Card not found"
                    };
            }

            //-----------------------------------
            // Covered
            //-----------------------------------

            if (candidates.Count == 1)
            {
                var cardFound =
                    candidates.First();

                return
                    new Data_output_catalog_result
                    {
                        status =
                            "COVERED",

                        message =
                            "Card found",

                        card_id =
                            cardFound.card_id,

                        set_name =
                            cardFound.set_name,

                        card_number =
                            cardFound.card_number,

                        edition =
                            cardFound.edition,

                        language =
                            cardFound.language,

                        finish_type =
                            cardFound.finish_type,

                        candidates =
                            new List<
                                Data_output_catalog_candidate>()
                    };
            }

            //-----------------------------------
            // Multiple match
            //-----------------------------------

            return
                new Data_output_catalog_result
                {
                    status =
                        "MULTIPLE_MATCH",

                    message =
                        "Multiple matches found",

                    candidates =
                        candidates
                };
        }

        public void SaveRequest(
            Guid requestId,
            Guid apiKeyId,
            string externalRequestId,
            string responseJson)
        {
            _databaseService.ExecuteNonQuery(
            @"
            INSERT INTO API_REQUESTS
            (
                request_id,
                api_key_id,
                external_request_id,
                response_json,
                created_at
            )
            VALUES
            (
                @RequestId,
                @ApiKeyId,
                @ExternalRequestId,
                @ResponseJson,
                GETUTCDATE()
            )
            ",
            new Dictionary<string, object>
            {
                { "RequestId", requestId },

                { "ApiKeyId", apiKeyId },

                {
                    "ExternalRequestId",
                    externalRequestId
                },

                {
                    "ResponseJson",
                    responseJson
                }
            });
        }

        public void SaveAudit(
            Guid apiKeyId,
            Guid requestId,
            int cardsCount)
        {
            _databaseService.ExecuteNonQuery(
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
            new Dictionary<string, object>
            {
                {
                    "AuditId",
                    Guid.NewGuid()
                },

                {
                    "ApiKeyId",
                    apiKeyId
                },

                {
                    "RequestId",
                    requestId
                },

                {
                    "CardsCount",
                    cardsCount
                }
            });
        }

        public dynamic GetExistingRequest(
            Guid apiKeyId,
            string externalRequestId)
        {
            return _databaseService
                .QuerySingleOrDefault<dynamic>(
                @"
                SELECT *
                FROM API_REQUESTS
                WHERE
                    api_key_id =
                        @ApiKeyId

                    AND
                    external_request_id =
                        @ExternalRequestId
                ",
                new Dictionary<string, object>
                {
                    {
                        "ApiKeyId",
                        apiKeyId
                    },

                    {
                        "ExternalRequestId",
                        externalRequestId
                    }
                });
        }
    }
}