using Tesseract;

namespace PokeGrading.Services
{
    public interface IOcrService
    {
        Task<string> ExtractTextAsync(string imagePath);
    }

    public class OcrService : IOcrService
    {
        public async Task<string> ExtractTextAsync(string imagePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
                    using var img = Pix.LoadFromFile(imagePath);
                    using var page = engine.Process(img);
                    
                    string text = page.GetText();
                    return text.Replace("\r", " ").Replace("\n", " ").Trim();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to extract text from image using OCR", ex);
                }
            });
        }
    }
}
