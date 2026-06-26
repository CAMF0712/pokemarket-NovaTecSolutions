// Contrato de servicio: define capacidades de negocio para IImageStorageService.
using Microsoft.AspNetCore.Http;

namespace PokeGrading.Services
{
    /// <summary>
    /// Contrato de almacenamiento de imagenes temporales y definitivas.
    /// </summary>
    public interface IImageStorageService
    {
        /// <summary>
        /// Guarda imagenes finales de carta y devuelve sus rutas publicas.
        /// </summary>
        Task<StoredCardImagesResult> SaveCardImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage);

        /// <summary>
        /// Guarda temporalmente una imagen para el flujo de busqueda.
        /// </summary>
        Task<TemporaryImageReference> SaveTemporarySearchImageAsync(IFormFile image);

        /// <summary>
        /// Guarda temporalmente imagenes de grading para su procesamiento posterior.
        /// </summary>
        Task<TemporaryGradingImagesResult> SaveTemporaryGradingImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage);

        /// <summary>
        /// Mueve imagenes temporales de grading al directorio final.
        /// </summary>
        void MoveGradingImagesToFinal(
            TemporaryImageReference frontImage,
            TemporaryImageReference? backImage);

        /// <summary>
        /// Elimina archivos del disco ignorando rutas vacias o inexistentes.
        /// </summary>
        void DeleteFilesIfExist(params string?[] filePaths);
    }
}
