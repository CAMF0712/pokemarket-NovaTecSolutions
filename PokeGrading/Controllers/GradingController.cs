using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradingController : ControllerBase
    {
        private readonly DatabaseService _database;

        private readonly IWebHostEnvironment _environment;

        public GradingController(
            DatabaseService database,
            IWebHostEnvironment environment)
        {
            _database = database;
            _environment = environment;
        }

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
                _database.QuerySingleOrDefault<Guid?>
                (
                    @"
                    SELECT user_id
                    FROM USERS
                    WHERE user_id=@user_id
                    ",
                    new()
                    {
                        {"user_id", input.user_id}
                    }
                );

            if (user == null)
            {
                return BadRequest(
                    "User not found");
            }

            //-----------------------------------
            // Card Exists
            //-----------------------------------

            var card =
                _database.QuerySingleOrDefault<Guid?>
                (
                    @"
                    SELECT card_id
                    FROM CARDS
                    WHERE card_id=@card_id
                    ",
                    new()
                    {
                        {"card_id", input.card_id}
                    }
                );

            if (card == null)
            {
                return BadRequest(
                    "Card not found");
            }

            //-----------------------------------
            // Image Validation
            //-----------------------------------

            if (!ImageValidationService
                .IsValidImage(
                    input.front_image))
            {
                return BadRequest(
                    "Front image invalid");
            }

            if (input.back_image != null)
            {
                if (!ImageValidationService
                    .IsValidImage(
                        input.back_image))
                {
                    return BadRequest(
                        "Back image invalid");
                }
            }

            //-----------------------------------
            // Folder
            //-----------------------------------

            string gradingFolder =
                Path.Combine(
                    _environment.ContentRootPath,
                    "GradingImages");

            Directory.CreateDirectory(
                gradingFolder);

            //-----------------------------------
            // Save Front
            //-----------------------------------

            string frontName =
                $"{Guid.NewGuid()}_front" +
                Path.GetExtension(
                    input.front_image.FileName);

            string frontPath =
                Path.Combine(
                    gradingFolder,
                    frontName);

            using (var stream =
                new FileStream(
                    frontPath,
                    FileMode.Create))
            {
                await input.front_image
                    .CopyToAsync(stream);
            }

            //-----------------------------------
            // Save Back
            //-----------------------------------

            string? backName = null;

            if (input.back_image != null)
            {
                backName =
                    $"{Guid.NewGuid()}_back" +
                    Path.GetExtension(
                        input.back_image.FileName);

                string backPath =
                    Path.Combine(
                        gradingFolder,
                        backName);

                using (var stream =
                    new FileStream(
                        backPath,
                        FileMode.Create))
                {
                    await input.back_image
                        .CopyToAsync(stream);
                }
            }

            //-----------------------------------
            // Vision Processing
            //-----------------------------------

            decimal confidence =
                GradingVisionService
                    .CalculateConfidence(
                        input.front_image,
                        input.back_image);

            decimal estimatedGrade =
                GradingVisionService
                    .EstimateGrade(
                        input.front_image,
                        input.back_image);

            //-----------------------------------
            // Status
            //-----------------------------------

            string status =
                confidence >= 85
                ?
                "COMPLETED"
                :
                "PENDING_REVIEW";

            //-----------------------------------
            // Current Algorithm Version
            //-----------------------------------

            Guid versionId =
                _database.QuerySingle<Guid>(
                @"
                SELECT TOP 1 version_id
                FROM ALGORITHM_VERSIONS
                WHERE active = 1
                ",
                new());

            //-----------------------------------
            // IDs
            //-----------------------------------

            Guid gradingId =
                Guid.NewGuid();

            Guid subgradeId =
                Guid.NewGuid();

            //-----------------------------------
            // Insert Grading
            //-----------------------------------

            _database.ExecuteNonQuery(
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
            new()
            {
                {"grading_id", gradingId},
                {"user_id", input.user_id},
                {"card_id", input.card_id},
                {"version_id", versionId},
                {"estimated_grade", estimatedGrade},
                {"confidence_score", confidence},
                {"status", status}
            });

            //-----------------------------------
            // Subgrades
            //-----------------------------------

            _database.ExecuteNonQuery(
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
            new()
            {
                {"subgrade_id", subgradeId},
                {"grading_id", gradingId},
                {"centering", GradingVisionService.CalculateCentering()},
                {"corners", GradingVisionService.CalculateCorners()},
                {"edges", GradingVisionService.CalculateEdges()},
                {"surface", GradingVisionService.CalculateSurface()}
            });

            //-----------------------------------
            // Front Image
            //-----------------------------------

            _database.ExecuteNonQuery(
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
            new()
            {
                {"image_id", Guid.NewGuid()},
                {"grading_id", gradingId},
                {"image_url", frontName}
            });

            //-----------------------------------
            // Back Image
            //-----------------------------------

            if (backName != null)
            {
                _database.ExecuteNonQuery(
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
                new()
                {
                    {"image_id", Guid.NewGuid()},
                    {"grading_id", gradingId},
                    {"image_url", backName}
                });
            }

            //-----------------------------------
            // Response
            //-----------------------------------

            return Ok(
                new Data_response<
                    Data_output_submit_grading>
                {
                    status = true,

                    data =
                        new Data_output_submit_grading
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