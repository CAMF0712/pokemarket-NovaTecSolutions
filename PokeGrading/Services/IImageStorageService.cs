using Microsoft.AspNetCore.Http;

namespace PokeGrading.Services
{
    public interface IImageStorageService
    {
        Task<StoredCardImagesResult> SaveCardImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage);

        Task<TemporaryImageReference> SaveTemporarySearchImageAsync(IFormFile image);

        Task<TemporaryGradingImagesResult> SaveTemporaryGradingImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage);

        void MoveGradingImagesToFinal(
            TemporaryImageReference frontImage,
            TemporaryImageReference? backImage);

        void DeleteFilesIfExist(params string?[] filePaths);
    }
}