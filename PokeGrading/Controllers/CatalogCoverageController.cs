using Microsoft.AspNetCore.Mvc;

using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("api/b2b/catalog")]
    public class CatalogCoverageController
        : ControllerBase
    {
        private readonly
            ApiKeyValidationService
            _apiKeyValidationService;

        private readonly
            CatalogCoverageService
            _catalogCoverageService;

        public CatalogCoverageController(
            ApiKeyValidationService
                apiKeyValidationService,

            CatalogCoverageService
                catalogCoverageService)
        {
            _apiKeyValidationService =
                apiKeyValidationService;

            _catalogCoverageService =
                catalogCoverageService;
        }

        [HttpPost("coverage")]
        public ActionResult<
            Data_response<
                Data_output_catalog_coverage>>
            Coverage(
                [FromBody]
                Data_input_catalog_coverage
                input)
        {
            try
            {
                //-----------------------------------
                // Trace Id
                //-----------------------------------

                string traceId =
                    Guid.NewGuid()
                    .ToString();

                //-----------------------------------
                // Api Key Required
                //-----------------------------------

                if (string.IsNullOrWhiteSpace(
                    input.api_key))
                {
                    return Unauthorized(
                        new Data_output_api_error
                        {
                            error_code =
                                "INVALID_API_KEY",

                            message =
                                "API Key required",

                            trace_id =
                                traceId
                        });
                }

                //-----------------------------------
                // Hash API Key
                //-----------------------------------

                string apiKeyHash;

                using (MD5 md5 =
                    MD5.Create())
                {
                    byte[] hash =
                        md5.ComputeHash(
                            Encoding.UTF8.GetBytes(
                                input.api_key));

                    apiKeyHash =
                        BitConverter
                        .ToString(hash)
                        .Replace("-", "")
                        .ToLower();
                }

                //-----------------------------------
                // Validate Key
                //-----------------------------------

                var apiKey =
                    _apiKeyValidationService
                    .ValidateApiKey(
                        apiKeyHash);

                if (apiKey == null)
                {
                    return Unauthorized(
                        new Data_output_api_error
                        {
                            error_code =
                                "INVALID_API_KEY",

                            message =
                                "API Key not found",

                            trace_id =
                                traceId
                        });
                }

                if (
                    apiKey.status !=
                    "ACTIVE"
                )
                {
                    return Unauthorized(
                        new Data_output_api_error
                        {
                            error_code =
                                "API_KEY_DISABLED",

                            message =
                                "API Key inactive",

                            trace_id =
                                traceId
                        });
                }

                if (
                    apiKey.active == false
                )
                {
                    return Unauthorized(
                        new Data_output_api_error
                        {
                            error_code =
                                "CLIENT_DISABLED",

                            message =
                                "B2B Client inactive",

                            trace_id =
                                traceId
                        });
                }

                //-----------------------------------
                // Request Validation
                //-----------------------------------

                if (
                    input.cards == null
                    ||
                    input.cards.Count == 0
                )
                {
                    return BadRequest(
                        new Data_output_api_error
                        {
                            error_code =
                                "INVALID_REQUEST",

                            message =
                                "No cards supplied",

                            trace_id =
                                traceId
                        });
                }

                //-----------------------------------
                // Idempotency
                //-----------------------------------

                if (
                    !string.IsNullOrWhiteSpace(
                        input.external_request_id))
                {
                    var existing =
                        _catalogCoverageService
                        .GetExistingRequest(
                            apiKey.api_key_id,
                            input.external_request_id);

                    if (existing != null)
                    {
                        var existingResponse =
                            JsonSerializer
                            .Deserialize<
                            Data_output_catalog_coverage>
                            (
                                existing.response_json
                            );

                        return Ok(
                            new Data_response<
                            Data_output_catalog_coverage>
                            {
                                status = true,

                                data =
                                    existingResponse
                            });
                    }
                }

                //-----------------------------------
                // Process
                //-----------------------------------

                var results =
                    _catalogCoverageService
                    .ProcessCards(
                        input.cards);

                var response =
                    new Data_output_catalog_coverage
                    {
                        request_id =
                            Guid.NewGuid(),

                        generated_at =
                            DateTime.UtcNow,

                        results =
                            results
                    };

                //-----------------------------------
                // Save Request
                //-----------------------------------

                if (
                    !string.IsNullOrWhiteSpace(
                        input.external_request_id))
                {
                    _catalogCoverageService
                    .SaveRequest(
                        response.request_id,

                        apiKey.api_key_id,

                        input.external_request_id,

                        JsonSerializer
                        .Serialize(response)
                    );
                }

                //-----------------------------------
                // Save Audit
                //-----------------------------------

                _catalogCoverageService
                .SaveAudit(
                    apiKey.api_key_id,

                    response.request_id,

                    input.cards.Count
                );

                //-----------------------------------
                // Return
                //-----------------------------------

                return Ok(
                    new Data_response<
                        Data_output_catalog_coverage>
                    {
                        status = true,

                        data =
                            response
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,

                    new Data_output_api_error
                    {
                        error_code =
                            "SYSTEM_ERROR",

                        message =
                            ex.Message,

                        trace_id =
                            Guid.NewGuid()

                            .ToString()
                    });
            }
        }
    }
}