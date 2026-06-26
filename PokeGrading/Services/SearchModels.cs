namespace PokeGrading.Services
{
    public class CardAttributesFromOcr
    {
        public int? DetectedHp { get; set; }

        public string? DetectedCardNumber { get; set; }

        public string? DetectedType { get; set; }
    }

    public class ScoredCandidate
    {
        public required int Score { get; init; }

        public required dynamic Card { get; init; }
    }

    public class SearchResult
    {
        public required List<ScoredCandidate> ScoredCandidates { get; init; }

        public required ScoredCandidate? BestMatch { get; init; }

        public required double Confidence { get; init; }
    }
}
