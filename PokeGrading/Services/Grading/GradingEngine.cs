using PokeGrading.Models.Grading;

namespace PokeGrading.Services.Grading
{
    public class GradingEngine : IGradingEngine
    {
        private const decimal CompletedThreshold = 85m;

        public GradingComputationResult Calculate(CardFeatures features)
        {
            var metrics = new GradingMetrics
            {
                Centering = NormalizeScore(features.Centering),
                Corners = NormalizeScore(features.Corners),
                Edges = NormalizeScore(features.Edges),
                Surface = NormalizeScore(features.Surface)
            };

            decimal estimatedGrade =
                CalculateEstimatedGrade(metrics);

            decimal confidence =
                CalculateConfidence(features);

            return new GradingComputationResult
            {
                EstimatedGrade = estimatedGrade,

                ConfidenceScore = confidence,

                Status =
                    confidence >= CompletedThreshold
                        ? "COMPLETED"
                        : "PENDING_REVIEW",

                Metrics = metrics
            };
        }

        private static decimal CalculateEstimatedGrade(
            GradingMetrics metrics)
        {
            decimal grade =
                (
                    metrics.Centering +
                    metrics.Corners +
                    metrics.Edges +
                    metrics.Surface
                ) / 4m;

            return Math.Round(grade, 2);
        }

        private static decimal CalculateConfidence(
            CardFeatures features)
        {
            decimal confidence = 100m;

            confidence -=
                Math.Abs(features.Brightness - 128m) * 0.10m;

            confidence -=
                features.Whitening * 10m;

            confidence -=
                features.Scratches * 10m;

            confidence +=
                features.Sharpness * 0.01m;

            confidence =
                Math.Clamp(confidence, 0m, 100m);

            return Math.Round(confidence, 2);
        }

        private static decimal NormalizeScore(decimal value)
        {
            return Math.Clamp(
                Math.Round(value, 2),
                1m,
                10m);
        }
    }
}