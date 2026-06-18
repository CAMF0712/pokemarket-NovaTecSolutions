using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace PokeGrading.Utilities
{
    public static class ImageValidationService
    {
        private const int MinSupportedImageWidth = 600;
        private const int MinSupportedImageHeight = 600;

        public static bool IsValidImage(
            IFormFile file)
        {
            if (file == null)
                return false;

            try
            {
                using var image =
                    Image.Load(
                        file.OpenReadStream());

                return
                    image.Width >= MinSupportedImageWidth
                    &&
                    image.Height >= MinSupportedImageHeight;
            }
            catch
            {
                return false;
            }
        }
    }
}