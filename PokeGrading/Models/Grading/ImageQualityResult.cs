namespace PokeGrading.Models.Grading
{
    /// <summary>
    /// Resultado de la validación de calidad de una imagen.
    /// </summary>
    public class ImageQualityResult
    {
        public bool IsValid { get; set; }

        public string? ErrorMessage { get; set; }

        public double Brightness { get; set; }

        public double Sharpness { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}