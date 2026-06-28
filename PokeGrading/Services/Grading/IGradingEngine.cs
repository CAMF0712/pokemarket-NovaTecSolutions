using PokeGrading.Models.Grading;

namespace PokeGrading.Services.Grading
{
    /// <summary>
    /// Calcula el resultado final del grading a partir de las características extraídas.
    /// </summary>
    public interface IGradingEngine
    {
        GradingComputationResult Calculate(CardFeatures features);
    }
}