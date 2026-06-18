using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using Dapper;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradingController : ControllerBase
    {
        private const decimal ConfidenceThresholdForCompletedStatus = 85m;
        private const int ActiveAlgorithmFlag = 1;

        // Programación defensiva: límite explícito de tamaño de imagen (10 MB)
        private const long MaxImageSizeBytes = 10 * 1024 * 1024;

        // Programación defensiva: lista de extensiones de imagen permitidas
        private static readonly string[] AllowedImageExtensions =
            { ".jpg", ".jpeg", ".png" };

        private readonly DatabaseService _database;
        private readonly IWebHostEnvironment _environment;

        public GradingController(
            DatabaseService database,
            IWebHostEnvironment environment)
        {
            _database = database;
            _environment = environment;
        }

        /// <summary>
        /// Recibe las imágenes de una carta, ejecuta el proceso de grading automático
        /// y persiste el resultado junto con sus subgrades en la base de datos.
        /// Usa una transacción para garantizar consistencia entre todas las inserciones.
        /// </summary>
        [HttpPost("submit")]
        public async Task<ActionResult<
            Data_response<Data_output_submit_grading>>>
            SubmitGrading(
                [FromForm]
                Data_input_submit_grading input)
        {
            //-----------------------------------
            // User Exists
            //-----------------------------------

            var user =
                _database.QuerySingleOrDefault<Guid?>(
                    @"
                    SELECT user_id
                    FROM USERS
                    WHERE user_id=@user_id
                    ",
                    new()
                    {
                        {"user_id", input.user_id}
                    });

            if (user == null)
            {
                return BadRequest("User not found");
            }

            //-----------------------------------
            // Card Exists
            //-----------------------------------

            var card =
                _database.QuerySingleOrDefault<Guid?>(
                    @"
                    SELECT card_id
                    FROM CARDS
                    WHERE card_id=@card_id
                    ",
                    new()
                    {
                        {"card_id", input.card_id}
                    });

            if (card == null)
            {
                return BadRequest("Card not found");
            }

            //-----------------------------------
            // Image Validation (formato y resolución)
            //-----------------------------------

            if (!ImageValidationService.IsValidImage(input.front_image))
            {
                return BadRequest("Front image invalid");
            }

            if (input.back_image != null)
            {
                if (!ImageValidationService.IsValidImage(input.back_image))
                {
                    return BadRequest("Back image invalid");
                }
            }

            //-----------------------------------
            // Programación defensiva: validar tamaño de imagen
            //-----------------------------------

            if (input.front_image.Length > MaxImageSizeBytes)
            {
                return BadRequest("Front image exceeds maximum allowed size of 10 MB");
            }

            if (input.back_image != null && input.back_image.Length > MaxImageSizeBytes)
            {
                return BadRequest("Back image exceeds maximum allowed size of 10 MB");
            }

            //-----------------------------------
            // Programación defensiva: validar extensión de archivo
            //-----------------------------------

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

            //-----------------------------------
            // Carpetas
            //-----------------------------------

            string gradingFolder =
                Path.Combine(_environment.ContentRootPath, "GradingImages");

            // Carpeta temporal: las imágenes se guardan aquí primero y
            // se mueven a la carpeta definitiva solo si la transacción de DB es exitosa
            string tempFolder =
                Path.Combine(_environment.ContentRootPath, "GradingImages", "temp");

            Directory.CreateDirectory(gradingFolder);
            Directory.CreateDirectory(tempFolder);

            //-----------------------------------
            // Guardar imágenes en carpeta temporal
            //-----------------------------------

            string frontTempName =
                $"{Guid.NewGuid()}_front{frontExt}";

            string frontTempPath = Path.Combine(tempFolder, frontTempName);

            using (var stream = new FileStream(frontTempPath, FileMode.Create))
            {
                await input.front_image.CopyToAsync(stream);
            }

            string? backTempName = null;
            string? backTempPath = null;

            if (input.back_image != null)
            {
                string backExt =
                    Path.GetExtension(input.back_image.FileName).ToLowerInvariant();

                backTempName = $"{Guid.NewGuid()}_back{backExt}";
                backTempPath = Path.Combine(tempFolder, backTempName);

                using (var stream = new FileStream(backTempPath, FileMode.Create))
                {
                    await input.back_image.CopyToAsync(stream);
                }
            }

            //-----------------------------------
            // Vision Processing
            //-----------------------------------

            decimal confidence =
                GradingVisionService.CalculateConfidence(
                    input.front_image,
                    input.back_image);

            decimal estimatedGrade =
                GradingVisionService.EstimateGrade(
                    input.front_image,
                    input.back_image);

            //-----------------------------------
            // Status
            //-----------------------------------

            string status =
                confidence >= ConfidenceThresholdForCompletedStatus
                    ? "COMPLETED"
                    : "PENDING_REVIEW";

            //-----------------------------------
            // Current Algorithm Version
            //-----------------------------------

            Guid versionId =
                _database.QuerySingle<Guid>(
                    @"
                SELECT TOP 1 version_id
                FROM ALGORITHM_VERSIONS
                WHERE active = @active
                ",
                    new()
                    {
                        {"active", ActiveAlgorithmFlag}
                    });

            //-----------------------------------
            // IDs
            //-----------------------------------

            Guid gradingId = Guid.NewGuid();
            Guid subgradeId = Guid.NewGuid();

            // Nombres definitivos de las imágenes (mismos que los temporales)
            string frontFinalName = frontTempName;
            string? backFinalName = backTempName;

            //-----------------------------------
            // Programación defensiva: todas las inserciones dentro de una transacción.
            // Si cualquier insert falla se hace rollback y se limpian los archivos temporales.
            //-----------------------------------

            try
            {
                _database.ExecuteInTransaction((conn, tx) =>
                {
                    conn.Execute(
                        @"
                        INSERT INTO GRADING
                        (
                            grading_id,
                            user_id,
                            card_id,
                            version_id,
                            estimated_grade,
                            confidence_score,
                            recommendation,
                            status,
                            created_at
                        )
                        VALUES
                        (
                            @grading_id,
                            @user_id,
                            @card_id,
                            @version_id,
                            @estimated_grade,
                            @confidence_score,
                            'AUTO_GENERATED',
                            @status,
                            GETUTCDATE()
                        )
                        ",
                        new
                        {
                            grading_id = gradingId,
                            user_id = input.user_id,
                            card_id = input.card_id,
                            version_id = versionId,
                            estimated_grade = estimatedGrade,
                            confidence_score = confidence,
                            status
                        },
                        transaction: tx);

                    conn.Execute(
                        @"
                        INSERT INTO SUBGRADES
                        (
                            subgrade_id,
                            grading_id,
                            centering,
                            corners,
                            edges,
                            surface
                        )
                        VALUES
                        (
                            @subgrade_id,
                            @grading_id,
                            @centering,
                            @corners,
                            @edges,
                            @surface
                        )
                        ",
                        new
                        {
                            subgrade_id = subgradeId,
                            grading_id = gradingId,
                            centering = GradingVisionService.CalculateCentering(),
                            corners = GradingVisionService.CalculateCorners(),
                            edges = GradingVisionService.CalculateEdges(),
                            surface = GradingVisionService.CalculateSurface()
                        },
                        transaction: tx);

                    conn.Execute(
                        @"
                        INSERT INTO GRADING_IMAGES
                        (
                            grading_image_id,
                            grading_id,
                            image_type,
                            image_url
                        )
                        VALUES
                        (
                            @image_id,
                            @grading_id,
                            'FRONT',
                            @image_url
                        )
                        ",
                        new
                        {
                            image_id = Guid.NewGuid(),
                            grading_id = gradingId,
                            image_url = frontFinalName
                        },
                        transaction: tx);

                    if (backFinalName != null)
                    {
                        conn.Execute(
                            @"
                            INSERT INTO GRADING_IMAGES
                            (
                                grading_image_id,
                                grading_id,
                                image_type,
                                image_url
                            )
                            VALUES
                            (
                                @image_id,
                                @grading_id,
                                'BACK',
                                @image_url
                            )
                            ",
                            new
                            {
                                image_id = Guid.NewGuid(),
                                grading_id = gradingId,
                                image_url = backFinalName
                            },
                            transaction: tx);
                    }
                });
            }
            catch
            {
                // Programación defensiva: si la transacción falló,
                // limpiar los archivos temporales para no dejar huérfanos en disco
                if (System.IO.File.Exists(frontTempPath))
                    System.IO.File.Delete(frontTempPath);

                if (backTempPath != null && System.IO.File.Exists(backTempPath))
                    System.IO.File.Delete(backTempPath);

                return StatusCode(500, "An error occurred while saving the grading. Please try again.");
            }

            //-----------------------------------
            // Transacción exitosa: mover archivos de temp a carpeta definitiva
            //-----------------------------------

            System.IO.File.Move(
                frontTempPath,
                Path.Combine(gradingFolder, frontFinalName));

            if (backTempPath != null && backFinalName != null)
            {
                System.IO.File.Move(
                    backTempPath,
                    Path.Combine(gradingFolder, backFinalName));
            }

            //-----------------------------------
            // Response
            //-----------------------------------

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
                        status = status
                    }
                });
        }
    }
}