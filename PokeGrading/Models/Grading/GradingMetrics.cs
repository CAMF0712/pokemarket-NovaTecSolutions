namespace PokeGrading.Models.Grading
{
    /// <summary>
    /// Subgrados calculados para una carta.
    /// </summary>
    public class GradingMetrics
    {
        public decimal Centering { get; set; }

        public decimal Corners { get; set; }

        public decimal Edges { get; set; }

        public decimal Surface { get; set; }
    }
}