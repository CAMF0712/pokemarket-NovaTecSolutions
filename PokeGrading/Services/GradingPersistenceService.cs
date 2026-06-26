using Dapper;
using PokeGrading.Utilities;

namespace PokeGrading.Services
{
    public class GradingPersistenceService : IGradingPersistenceService
    {
        private const int ActiveAlgorithmFlag = 1;

        private readonly DatabaseService _database;

        public GradingPersistenceService(DatabaseService database)
        {
            _database = database;
        }

        public Guid SaveGrading(GradingPersistenceRequest request)
        {
            Guid versionId = _database.QuerySingle<Guid>(
                @"
                SELECT TOP 1 version_id
                FROM ALGORITHM_VERSIONS
                WHERE active = @active
                ",
                new()
                {
                    { "active", ActiveAlgorithmFlag }
                });

            Guid gradingId = Guid.NewGuid();
            Guid subgradeId = Guid.NewGuid();

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
                        user_id = request.UserId,
                        card_id = request.CardId,
                        version_id = versionId,
                        estimated_grade = request.EstimatedGrade,
                        confidence_score = request.ConfidenceScore,
                        status = request.Status
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
                        centering = request.Centering,
                        corners = request.Corners,
                        edges = request.Edges,
                        surface = request.Surface
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
                        image_url = request.FrontImageUrl
                    },
                    transaction: tx);

                if (request.BackImageUrl != null)
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
                            image_url = request.BackImageUrl
                        },
                        transaction: tx);
                }
            });

            return gradingId;
        }
    }
}
