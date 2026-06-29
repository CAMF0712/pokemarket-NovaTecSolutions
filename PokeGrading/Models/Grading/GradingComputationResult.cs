namespace PokeGrading.Models.Grading
{
    /// <summary>
    /// Resultado final del algoritmo de grading.
    /// </summary>
    public class GradingComputationResult
    {
        public decimal EstimatedGrade { get; set; }

        public decimal ConfidenceScore { get; set; }

        public string Status { get; set; } = string.Empty;

        public required GradingMetrics Metrics { get; set; }
    }
}