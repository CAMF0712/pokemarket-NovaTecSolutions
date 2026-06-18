using Microsoft.AspNetCore.Http;

namespace PokeGrading.Utilities
{
    public static class GradingVisionService
    {
        private const decimal MinimumConfidenceScore = 80m;
        private const decimal ConfidenceScoreRange = 20m;
        private const int RoundingDecimals = 2;

        private const decimal MinimumEstimatedGrade = 7m;
        private const decimal EstimatedGradeRange = 3m;

        private const decimal DefaultCenteringSubgrade = 8.5m;
        private const decimal DefaultCornersSubgrade = 8.0m;
        private const decimal DefaultEdgesSubgrade = 8.5m;
        private const decimal DefaultSurfaceSubgrade = 9.0m;

        public static decimal CalculateConfidence(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            Random random = new Random();

            return Math.Round(
                MinimumConfidenceScore +
                (decimal)random.NextDouble() * ConfidenceScoreRange,
                RoundingDecimals);
        }

        public static decimal EstimateGrade(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            Random random = new Random();

            return Math.Round(
                MinimumEstimatedGrade +
                (decimal)random.NextDouble() * EstimatedGradeRange,
                RoundingDecimals);
        }

        public static decimal CalculateCentering()
        {
            return DefaultCenteringSubgrade;
        }

        public static decimal CalculateCorners()
        {
            return DefaultCornersSubgrade;
        }

        public static decimal CalculateEdges()
        {
            return DefaultEdgesSubgrade;
        }

        public static decimal CalculateSurface()
        {
            return DefaultSurfaceSubgrade;
        }
    }
}