using OpenCvSharp;
using PokeGrading.Models.Grading;

namespace PokeGrading.Services.FeatureExtraction
{
    /// <summary>
    /// Extrae características visuales de una carta.
    /// </summary>
    public interface IFeatureExtractionService
    {
        CardFeatures Extract(
            ImagePreprocessingResult images);
    }
}