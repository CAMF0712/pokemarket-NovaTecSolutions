namespace PokeGrading.Models.Grading
{
    /// <summary>
    /// Características visuales extraídas de la carta.
    /// </summary>
    public class CardFeatures
    {
        public decimal Centering { get; set; }

        public decimal Corners { get; set; }

        public decimal Edges { get; set; }

        public decimal Surface { get; set; }

        public decimal Brightness { get; set; }

        public decimal Sharpness { get; set; }

        public decimal Whitening { get; set; }

        public decimal Scratches { get; set; }

        public decimal Dents { get; set; }
    }
}