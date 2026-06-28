using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Models.Grading;
using PokeGrading.Repositories;
using PokeGrading.Services.Application;
using PokeGrading.Services.FeatureExtraction;
using PokeGrading.Services.Grading;
using PokeGrading.Services.ImagePreprocessing;
using PokeGrading.Utilities;

namespace PokeGrading.Services
{
    /// <summary>
    /// Orquesta el caso de uso completo del grading.
    /// </summary>
    public class GradingApplicationService
        : IGradingApplicationService
    {
        private const long MaxImageSizeBytes =
            10 * 1024 * 1024;

        private static readonly string[] AllowedImageExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        private readonly IUserRepository _userRepository;

        private readonly ICardRepository _cardRepository;

        private readonly IImageStorageService _imageStorageService;

        private readonly IGradingPersistenceService _gradingPersistenceService;

        private readonly IImagePreprocessingService _preprocessingService;

        private readonly IFeatureExtractionService _featureExtractionService;

        private readonly IGradingEngine _gradingEngine;

        public GradingApplicationService(
            IUserRepository userRepository,
            ICardRepository cardRepository,
            IImageStorageService imageStorageService,
            IGradingPersistenceService gradingPersistenceService,
            IImagePreprocessingService preprocessingService,
            IFeatureExtractionService featureExtractionService,
            IGradingEngine gradingEngine)
        {
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _imageStorageService = imageStorageService;
            _gradingPersistenceService = gradingPersistenceService;
            _preprocessingService = preprocessingService;
            _featureExtractionService = featureExtractionService;
            _gradingEngine = gradingEngine;
        }

        public async Task<ActionResult<Data_response<Data_output_submit_grading>>>
            SubmitGrading(
                Data_input_submit_grading input)
        {
            //----------------------------------------------------------
            // Validaciones
            //----------------------------------------------------------

            if (!_userRepository.UserExists(input.user_id))
            {
                return new BadRequestObjectResult("User not found");
            }

            if (!_cardRepository.CardExists(input.card_id))
            {
                return new BadRequestObjectResult("Card not found");
            }

            if (!ImageValidationService.IsValidImage(input.front_image))
            {
                return new BadRequestObjectResult("Front image invalid");
            }

            if (input.back_image != null &&
                !ImageValidationService.IsValidImage(input.back_image))
            {
                return new BadRequestObjectResult("Back image invalid");
            }

            if (input.front_image.Length > MaxImageSizeBytes)
            {
                return new BadRequestObjectResult(
                    "Front image exceeds maximum allowed size.");
            }

            if (input.back_image != null &&
                input.back_image.Length > MaxImageSizeBytes)
            {
                return new BadRequestObjectResult(
                    "Back image exceeds maximum allowed size.");
            }

            string frontExtension =
                Path.GetExtension(
                    input.front_image.FileName)
                .ToLowerInvariant();

            if (!AllowedImageExtensions.Contains(frontExtension))
            {
                return new BadRequestObjectResult(
                    "Front image format not supported.");
            }

            if (input.back_image != null)
            {
                string backExtension =
                    Path.GetExtension(
                        input.back_image.FileName)
                    .ToLowerInvariant();

                if (!AllowedImageExtensions.Contains(backExtension))
                {
                    return new BadRequestObjectResult(
                        "Back image format not supported.");
                }
            }



            //----------------------------------------------------------
            // Guardado temporal
            //----------------------------------------------------------

            var temporaryImages =
                await _imageStorageService
                    .SaveTemporaryGradingImagesAsync(
                        input.front_image,
                        input.back_image);

            try
            {
                //------------------------------------------------------
                // PREPROCESSING
                //------------------------------------------------------

                ImagePreprocessingResult processedImages =
                    _preprocessingService.Process(
                        temporaryImages.FrontImage.FilePath,
                        temporaryImages.BackImage?.FilePath);

                //------------------------------------------------------
                // FEATURE EXTRACTION
                //------------------------------------------------------

                CardFeatures features =
                    _featureExtractionService.Extract(
                        processedImages);

                //------------------------------------------------------
                // IMAGE QUALITY VALIDATION
                //------------------------------------------------------

                const decimal MinimumSharpness = 80m;

                const decimal MinimumBrightness = 40m;

                const decimal MaximumBrightness = 220m;

                if (features.Sharpness < MinimumSharpness)
                {
                    _imageStorageService.DeleteFilesIfExist(
                        temporaryImages.FrontImage.FilePath,
                        temporaryImages.BackImage?.FilePath);

                    return new BadRequestObjectResult(
                        "Front image is too blurry. Please capture the image again.");
                }

                if (features.Brightness < MinimumBrightness)
                {
                    _imageStorageService.DeleteFilesIfExist(
                        temporaryImages.FrontImage.FilePath,
                        temporaryImages.BackImage?.FilePath);

                    return new BadRequestObjectResult(
                        "Image lighting is too dark.");
                }

                if (features.Brightness > MaximumBrightness)
                {
                    _imageStorageService.DeleteFilesIfExist(
                        temporaryImages.FrontImage.FilePath,
                        temporaryImages.BackImage?.FilePath);

                    return new BadRequestObjectResult(
                        "Image lighting is too bright.");
                }

                //------------------------------------------------------
                // GRADING ENGINE
                //------------------------------------------------------

                GradingComputationResult grading =
                    _gradingEngine.Calculate(
                        features);

                //------------------------------------------------------
                // PERSISTENCIA
                //------------------------------------------------------

                Guid gradingId =
                    _gradingPersistenceService.SaveGrading(
                        new GradingPersistenceRequest
                        {
                            UserId = input.user_id,

                            CardId = input.card_id,

                            EstimatedGrade =
                                grading.EstimatedGrade,

                            ConfidenceScore =
                                grading.ConfidenceScore,

                            Status =
                                grading.Status,

                            Centering =
                                grading.Metrics.Centering,

                            Corners =
                                grading.Metrics.Corners,

                            Edges =
                                grading.Metrics.Edges,

                            Surface =
                                grading.Metrics.Surface,

                            FrontImageUrl =
                                temporaryImages.FrontImage.FileName,

                            BackImageUrl =
                                temporaryImages.BackImage?.FileName
                        });

                //------------------------------------------------------
                // Mover imágenes
                //------------------------------------------------------

                _imageStorageService.MoveGradingImagesToFinal(
                    temporaryImages.FrontImage,
                    temporaryImages.BackImage);

                //------------------------------------------------------
                // Respuesta
                //------------------------------------------------------

                return new OkObjectResult(
                    new Data_response<Data_output_submit_grading>
                    {
                        status = true,

                        data =
                            new Data_output_submit_grading
                            {
                                grading_id =
                                    gradingId,

                                card_id =
                                    input.card_id,

                                estimated_grade =
                                    grading.EstimatedGrade,

                                confidence_score =
                                    grading.ConfidenceScore,

                                status =
                                    grading.Status,

                                centering =
                                    grading.Metrics.Centering,

                                corners =
                                    grading.Metrics.Corners,

                                edges =
                                    grading.Metrics.Edges,

                                surface =
                                    grading.Metrics.Surface,

                                front_image_url =
                                    "/grading-images/" +
                                    temporaryImages.FrontImage.FileName,

                                back_image_url =
                                    temporaryImages.BackImage == null
                                        ? null
                                        : "/grading-images/" +
                                          temporaryImages.BackImage.FileName


                            }
                    });
            }
            catch
            {
                _imageStorageService.DeleteFilesIfExist(
                    temporaryImages.FrontImage.FilePath,
                    temporaryImages.BackImage?.FilePath);

                return new ObjectResult(
                    "An error occurred while processing the grading.")
                {
                    StatusCode = 500
                };
            }
        }
    }
}