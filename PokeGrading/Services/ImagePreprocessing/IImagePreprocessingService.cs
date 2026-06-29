using OpenCvSharp;
using PokeGrading.Models.Grading;

namespace PokeGrading.Services.ImagePreprocessing
{
    /// <summary>
    /// Define las operaciones de preprocesamiento de imágenes para grading.
    /// </summary>
    public interface IImagePreprocessingService
    {
        /// <summary>
        /// Preprocesa las imágenes frontal y trasera para dejarlas listas para visión computacional.
        /// </summary>
        ImagePreprocessingResult Process(
            string frontImagePath,
            string? backImagePath);
    }
}