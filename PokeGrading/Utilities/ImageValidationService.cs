using Microsoft.AspNetCore.Http;
using OpenCvSharp;

namespace PokeGrading.Utilities
{
    public static class ImageValidationService
    {
        private const int MinWidth = 1024;
        private const int MinHeight = 1024;

        private const int MaxWidth = 4096;
        private const int MaxHeight = 4096;

        public static bool IsValidImage(IFormFile file)
        {
            if (file == null)
                return false;

            try
            {
                using MemoryStream stream = new();

                file.CopyTo(stream);

                byte[] bytes = stream.ToArray();

                Mat image =
                    Cv2.ImDecode(
                        bytes,
                        ImreadModes.Color);

                if (image.Empty())
                    return false;

                return
                    image.Width >= MinWidth &&
                    image.Height >= MinHeight &&
                    image.Width <= MaxWidth &&
                    image.Height <= MaxHeight;
            }
            catch
            {
                return false;
            }
        }
    }
}