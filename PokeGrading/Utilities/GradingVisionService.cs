using Microsoft.AspNetCore.Http;

namespace PokeGrading.Utilities
{
    public static class GradingVisionService
    {
        public static decimal CalculateConfidence(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            Random random = new Random();

            return Math.Round(
                (decimal)(80 + random.NextDouble() * 20),
                2);
        }

        public static decimal EstimateGrade(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            Random random = new Random();

            return Math.Round(
                (decimal)(7 + random.NextDouble() * 3),
                2);
        }

        public static decimal CalculateCentering()
        {
            return 8.5m;
        }

        public static decimal CalculateCorners()
        {
            return 8.0m;
        }

        public static decimal CalculateEdges()
        {
            return 8.5m;
        }

        public static decimal CalculateSurface()
        {
            return 9.0m;
        }
    }
}