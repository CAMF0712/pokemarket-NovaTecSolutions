namespace PokeGrading.Utilities
{
    public class ImageService
    {
        public static bool ValidateImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return false;

            string lower =
                imageUrl.ToLower();

            return
                lower.EndsWith(".jpg")
                ||
                lower.EndsWith(".jpeg")
                ||
                lower.EndsWith(".png");
        }
    }
}