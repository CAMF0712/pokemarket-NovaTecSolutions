using OpenCvSharp;

namespace PokeGrading.Models.Grading
{
    /// <summary>
    /// Contiene las imágenes preprocesadas listas para la extracción
    /// de características visuales.
    /// </summary>
    public class ImagePreprocessingResult
    {
        /// <summary>
        /// Imagen frontal preprocesada.
        /// </summary>
        public required Mat FrontImage { get; init; }

        /// <summary>
        /// Imagen trasera preprocesada.
        /// </summary>
        public Mat? BackImage { get; init; }
    }
}