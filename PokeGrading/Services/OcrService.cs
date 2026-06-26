// Servicio: concentra logica de negocio y soporte para OcrService.
using Tesseract;

namespace PokeGrading.Services
{
    /// <summary>
    /// Contrato para ejecutar OCR sobre imagenes de cartas.
    /// </summary>
    public interface IOcrService
    {
        /// <summary>
        /// Extrae y normaliza el texto detectado en una imagen.
        /// </summary>
        Task<string> ExtractTextAsync(string imagePath);
    }

    /// <summary>
    /// Implementacion basada en Tesseract para reconocimiento de texto.
    /// </summary>
    public class OcrService : IOcrService
    {
        /// <summary>
        /// Ejecuta OCR sobre una imagen y devuelve el texto detectado.
        /// </summary>
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

