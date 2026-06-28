using OpenCvSharp;
using PokeGrading.Models.Grading;

namespace PokeGrading.Services.ImagePreprocessing
{
    public class ImagePreprocessingService
        : IImagePreprocessingService
    {
        public ImagePreprocessingResult Process(
            string frontImagePath,
            string? backImagePath)
        {
            Mat front = LoadAndPreprocess(frontImagePath);

            Mat? back = null;

            if (!string.IsNullOrWhiteSpace(backImagePath))
            {
                back = LoadAndPreprocess(backImagePath);
            }

            return new ImagePreprocessingResult
            {
                FrontImage = front,
                BackImage = back
            };
        }

        private static Mat LoadAndPreprocess(string path)
        {
            Mat image = Cv2.ImRead(path);

            image = NormalizeLighting(image);

            image = ReduceNoise(image);

            image = Resize(image);

            return image;
        }

        private static Mat NormalizeLighting(Mat image)
        {
            Mat lab = new();

            Cv2.CvtColor(image, lab, ColorConversionCodes.BGR2Lab);

            Mat[] channels = Cv2.Split(lab);

            Cv2.EqualizeHist(channels[0], channels[0]);

            Cv2.Merge(channels, lab);

            Mat result = new();

            Cv2.CvtColor(
                lab,
                result,
                ColorConversionCodes.Lab2BGR);

            return result;
        }

        private static Mat ReduceNoise(Mat image)
        {
            Mat result = new();

            Cv2.FastNlMeansDenoisingColored(
                image,
                result);

            return result;
        }

        private static Mat Resize(Mat image)
        {
            Mat result = new();

            Cv2.Resize(
                image,
                result,
                new Size(1024, 1024));

            return result;
        }
    }
}