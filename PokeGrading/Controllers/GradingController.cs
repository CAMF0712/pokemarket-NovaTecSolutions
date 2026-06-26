using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Repositories;
using PokeGrading.Services;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradingController : ControllerBase
    {
        private const decimal ConfidenceThresholdForCompletedStatus = 85m;

        private const long MaxImageSizeBytes = 10 * 1024 * 1024;

        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png" };

        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IGradingPersistenceService _gradingPersistenceService;

        public GradingController(
            ICardRepository cardRepository,
            IUserRepository userRepository,
            IImageStorageService imageStorageService,
            IGradingPersistenceService gradingPersistenceService)
        {
            _cardRepository = cardRepository;
            _userRepository = userRepository;
            _imageStorageService = imageStorageService;
            _gradingPersistenceService = gradingPersistenceService;
        }

        [HttpPost("submit")]
        public async Task<ActionResult<
            Data_response<Data_output_submit_grading>>>
            SubmitGrading(
                [FromForm]
                Data_input_submit_grading input)
        {
            this.EnsureTraceId();

            if (!_userRepository.UserExists(input.user_id))
            {
                return BadRequest("User not found");
            }

            if (!_cardRepository.CardExists(input.card_id))
            {
                return BadRequest("Card not found");
            }

            if (!ImageValidationService.IsValidImage(input.front_image))
            {
                return BadRequest("Front image invalid");
            }

            if (input.back_image != null &&
                !ImageValidationService.IsValidImage(input.back_image))
            {
                return BadRequest("Back image invalid");
            }

            if (input.front_image.Length > MaxImageSizeBytes)
            {
                return BadRequest("Front image exceeds maximum allowed size of 10 MB");
            }

            if (input.back_image != null && input.back_image.Length > MaxImageSizeBytes)
            {
                return BadRequest("Back image exceeds maximum allowed size of 10 MB");
            }

            string frontExt =
                Path.GetExtension(input.front_image.FileName)
                    .ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(frontExt))
            {
                return BadRequest(
                    $"Front image extension '{frontExt}' is not allowed. Accepted: jpg, jpeg, png");
            }

            if (input.back_image != null)
            {
                string backExt =
                    Path.GetExtension(input.back_image.FileName)
                        .ToLowerInvariant();

                if (!AllowedImageExtensions.Contains(backExt))
                {
                    return BadRequest(
                        $"Back image extension '{backExt}' is not allowed. Accepted: jpg, jpeg, png");
                }
            }

            var temporaryImages =
                await _imageStorageService
                    .SaveTemporaryGradingImagesAsync(
                        input.front_image,
                        input.back_image);

            decimal confidence =
                GradingVisionService.CalculateConfidence(
                    input.front_image,
                    input.back_image);

            decimal estimatedGrade =
                GradingVisionService.EstimateGrade(
                    input.front_image,
                    input.back_image);

            string gradingStatus =
                confidence >= ConfidenceThresholdForCompletedStatus
                    ? "COMPLETED"
                    : "PENDING_REVIEW";

            string frontFinalName = temporaryImages.FrontImage.FileName;
            string? backFinalName = temporaryImages.BackImage?.FileName;

            Guid gradingId;

            try
            {
                gradingId =
                    _gradingPersistenceService.SaveGrading(
                        new GradingPersistenceRequest
                        {
                            UserId = input.user_id,
                            CardId = input.card_id,
                            EstimatedGrade = estimatedGrade,
                            ConfidenceScore = confidence,
                            Status = gradingStatus,
                            Centering = GradingVisionService.CalculateCentering(),
                            Corners = GradingVisionService.CalculateCorners(),
                            Edges = GradingVisionService.CalculateEdges(),
                            Surface = GradingVisionService.CalculateSurface(),
                            FrontImageUrl = frontFinalName,
                            BackImageUrl = backFinalName
                        });
            }
            catch
            {
                _imageStorageService.DeleteFilesIfExist(
                    temporaryImages.FrontImage.FilePath,
                    temporaryImages.BackImage?.FilePath);

                return StatusCode(
                    500,
                    "An error occurred while saving the grading. Please try again.");
            }

            _imageStorageService.MoveGradingImagesToFinal(
                temporaryImages.FrontImage,
                temporaryImages.BackImage);

            return Ok(
                new Data_response<Data_output_submit_grading>
                {
                    status = true,
                    data = new Data_output_submit_grading
                    {
                        grading_id = gradingId,
                        card_id = input.card_id,
                        estimated_grade = estimatedGrade,
                        confidence_score = confidence,
                        status = gradingStatus
                    }
                });
        }
    }
}

