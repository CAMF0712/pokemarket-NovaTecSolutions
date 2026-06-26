namespace PokeGrading.Services
{
    public class GradingPersistenceRequest
    {
        public required Guid UserId { get; init; }

        public required Guid CardId { get; init; }

        public required decimal EstimatedGrade { get; init; }

        public required decimal ConfidenceScore { get; init; }

        public required string Status { get; init; }

        public required decimal Centering { get; init; }

        public required decimal Corners { get; init; }

        public required decimal Edges { get; init; }

        public required decimal Surface { get; init; }

        public required string FrontImageUrl { get; init; }

        public string? BackImageUrl { get; init; }
    }

    public interface IGradingPersistenceService
    {
        Guid SaveGrading(GradingPersistenceRequest request);
    }
}
