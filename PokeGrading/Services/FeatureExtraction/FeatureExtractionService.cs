using OpenCvSharp;
using PokeGrading.Models.Grading;

namespace PokeGrading.Services.FeatureExtraction
{
    public class FeatureExtractionService
        : IFeatureExtractionService
    {
        public CardFeatures Extract(
            ImagePreprocessingResult images)
        {
            var features = new CardFeatures();

            features.Brightness =
                (decimal)CalculateBrightness(images.FrontImage);

            features.Sharpness =
                (decimal)CalculateSharpness(images.FrontImage);

            features.Centering =
                (decimal)CalculateCentering(images.FrontImage);

            features.Whitening =
                (decimal)EstimateWhitening(images.FrontImage);

            features.Scratches =
                (decimal)EstimateScratches(images.FrontImage);

            features.Corners =
                (decimal)EstimateCorners(images.FrontImage);

            features.Edges =
                (decimal)EstimateEdges(images.FrontImage);

            features.Surface =
                CalculateSurface(features);

            return features;
        }

        /// <summary>
        /// Calcula el brillo de la imagen convirtiéndola a escala de grises y luego calculando el valor medio de los píxeles.
        /// </summary>
        private static double CalculateBrightness(Mat image)
        {
            Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            return Cv2.Mean(gray).Val0;
        }

        /// <summary>
        /// Calcula la nitidez de la imagen utilizando el operador Laplaciano y devuelve la varianza de la imagen resultante.
        /// </summary>
        private static double CalculateSharpness(Mat image)
        {
            Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            Mat laplacian = new();

            Cv2.Laplacian(
                gray,
                laplacian,
                MatType.CV_64F);

            Cv2.MeanStdDev(
                laplacian,
                out _,
                out Scalar stddev);

            return stddev.Val0 * stddev.Val0;
        }

        /// <summary>
        /// Calcula el centrado de la carta en la imagen detectando los contornos y encontrando el rectángulo delimitador más grande.
        /// Luego, calcula la distancia entre el centro de la imagen y el centro del rectángulo delimitador, y devuelve una puntuación basada en esa distancia.
        /// </summary>
        private static double CalculateCentering(Mat image)
        {
            using Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            // Reduce ruido antes de detectar bordes
            Cv2.GaussianBlur(
                gray,
                gray,
                new Size(5, 5),
                0);

            using Mat edges = new();

            Cv2.Canny(
                gray,
                edges,
                50,
                150);

            Cv2.FindContours(
                edges,
                out Point[][] contours,
                out _,
                RetrievalModes.External,
                ContourApproximationModes.ApproxSimple);

            if (contours.Length == 0)
            {
                return 5.0;
            }

            Rect card =
                contours
                    .Select(Cv2.BoundingRect)
                    .OrderByDescending(r => r.Width * r.Height)
                    .First();

            Point2d imageCenter =
                new(
                    image.Width / 2.0,
                    image.Height / 2.0);

            Point2d cardCenter =
                new(
                    card.X + card.Width / 2.0,
                    card.Y + card.Height / 2.0);

            double distance =
                Math.Sqrt(
                    Math.Pow(imageCenter.X - cardCenter.X, 2) +
                    Math.Pow(imageCenter.Y - cardCenter.Y, 2));

            double maxDistance =
                Math.Sqrt(
                    Math.Pow(image.Width / 2.0, 2) +
                    Math.Pow(image.Height / 2.0, 2));

            double score =
                10.0 * (1.0 - distance / maxDistance);

            return Math.Round(
                Math.Clamp(score, 1.0, 10.0),
                2);
        }

        /// <summary>
        /// Estima el nivel de blanqueamiento en la imagen convirtiéndola a escala de grises
        /// aplicando un umbral para identificar los píxeles blancos y calculando la proporción de píxeles blancos en relación con el total de píxeles.
        /// </summary>
        private static double EstimateWhitening(Mat image)
        {
            const int WhiteningThreshold = 235;

            using Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            using Mat threshold = new();

            Cv2.Threshold(
                gray,
                threshold,
                WhiteningThreshold,
                255,
                ThresholdTypes.Binary);

            int whitePixels =
                Cv2.CountNonZero(threshold);

            return
                (double)whitePixels /
                gray.Total();
        }

        /// <summary>
        /// Estima el nivel de arañazos en la imagen utilizando el detector de bordes Canny y calculando la densidad de arañazos (número de píxeles de borde dividido por el total de píxeles).
        /// </summary>
        private static double EstimateScratches(Mat image)
        {
            using Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            // Reduce ruido fino
            Cv2.MedianBlur(
                gray,
                gray,
                3);

            using Mat edges = new();

            Cv2.Canny(
                gray,
                edges,
                120,
                220);

            double scratchDensity =
                Cv2.CountNonZero(edges)
                / (double)edges.Total();

            return Math.Round(
                scratchDensity,
                4);
        }

        /// <summary>
        /// Estima el nivel de esquinas en la imagen utilizando el detector de esquinas Good Features to Track y contando el número de esquinas detectadas en relación con un valor máximo de 80.
        /// </summary>
        private static double EstimateCorners(Mat image)
        {
            Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            Point2f[] corners =
                Cv2.GoodFeaturesToTrack(
                    gray,
                    80,
                    0.01,
                    10,
                    null,
                    3,
                    false,
                    0.04);

            if (corners == null || corners.Length == 0)
            {
                return 1;
            }

            double score =
                Math.Min(
                    corners.Length / 8.0,
                    10.0);

            return Math.Round(score, 2);
        }

        /// <summary>
        /// Estima el nivel de bordes en la imagen utilizando el detector de bordes Canny y calculando la densidad de bordes (número de píxeles de borde dividido por el total de píxeles).
        /// Luego, se calcula una puntuación basada en la densidad de bordes, donde una mayor densidad de bordes resulta en una puntuación más baja. 
        /// La puntuación se limita a un rango de 1 a 10 y se redondea a dos decimales.
        /// </summary>
        private static double EstimateEdges(Mat image)
        {
            using Mat gray = new();

            Cv2.CvtColor(
                image,
                gray,
                ColorConversionCodes.BGR2GRAY);

            using Mat edges = new();

            Cv2.Canny(
                gray,
                edges,
                75,
                150);

            double density =
                Cv2.CountNonZero(edges) /
                (double)edges.Total();

            double score =
                10.0 - density * 18.0;

            return Math.Round(
                Math.Clamp(score, 1.0, 10.0),
                2);
        }

        /// <summary>
        /// Calcula la superficie de la tarjeta en función de las características extraídas. La superficie se calcula como 10 menos 5 veces el nivel de blanqueamiento 
        /// y menos 8 veces el nivel de arañazos. Si la superficie calculada es menor que 1, se establece en 1. Finalmente, se redondea a dos decimales.
        /// </summary>
        private static decimal CalculateSurface(CardFeatures features)
        {
            decimal surface = 10m;

            surface -= features.Whitening * 5m;

            surface -= features.Scratches * 8m;

            if (surface < 1)
                surface = 1;

            return Math.Round(surface, 2);
        }

    }
}