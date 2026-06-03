using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace PokeGrading.Utilities
{
    public static class ImageValidationService
    {
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
                    image.Width >= 600
                    &&
                    image.Height >= 600;
            }
            catch
            {
                return false;
            }
        }
    }
}