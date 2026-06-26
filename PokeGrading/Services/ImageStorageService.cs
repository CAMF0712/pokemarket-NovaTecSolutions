using Microsoft.AspNetCore.Http;

namespace PokeGrading.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly IWebHostEnvironment _environment;

        public ImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<StoredCardImagesResult> SaveCardImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            string imagesFolder = GetFolderPath("Images");

            Directory.CreateDirectory(imagesFolder);

            string frontFileName =
                $"{Guid.NewGuid()}{Path.GetExtension(frontImage.FileName)}";

            string frontPath = Path.Combine(imagesFolder, frontFileName);
            await SaveFileAsync(frontImage, frontPath);

            string? backImageUrl = null;

            if (backImage != null)
            {
                string backFileName =
                    $"{Guid.NewGuid()}{Path.GetExtension(backImage.FileName)}";

                string backPath = Path.Combine(imagesFolder, backFileName);
                await SaveFileAsync(backImage, backPath);

                backImageUrl = $"/images/{backFileName}";
            }

            return new StoredCardImagesResult
            {
                FrontImageUrl = $"/images/{frontFileName}",
                BackImageUrl = backImageUrl
            };
        }

        public async Task<TemporaryImageReference> SaveTemporarySearchImageAsync(IFormFile image)
        {
            string tempFolder = GetFolderPath("Temp");
            Directory.CreateDirectory(tempFolder);

            string extension = Path.GetExtension(image.FileName);

            if (string.IsNullOrWhiteSpace(extension))
            {
                extension = ".jpg";
            }

            string fileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(tempFolder, fileName);

            await SaveFileAsync(image, filePath);

            return new TemporaryImageReference
            {
                FileName = fileName,
                FilePath = filePath
            };
        }

        public async Task<TemporaryGradingImagesResult> SaveTemporaryGradingImagesAsync(
            IFormFile frontImage,
            IFormFile? backImage)
        {
            string tempFolder = GetFolderPath("GradingImages", "temp");
            Directory.CreateDirectory(tempFolder);

            string frontExt = Path.GetExtension(frontImage.FileName).ToLowerInvariant();
            string frontFileName = $"{Guid.NewGuid()}_front{frontExt}";
            string frontFilePath = Path.Combine(tempFolder, frontFileName);

            await SaveFileAsync(frontImage, frontFilePath);

            TemporaryImageReference? backReference = null;

            if (backImage != null)
            {
                string backExt = Path.GetExtension(backImage.FileName).ToLowerInvariant();
                string backFileName = $"{Guid.NewGuid()}_back{backExt}";
                string backFilePath = Path.Combine(tempFolder, backFileName);

                await SaveFileAsync(backImage, backFilePath);

                backReference = new TemporaryImageReference
                {
                    FileName = backFileName,
                    FilePath = backFilePath
                };
            }

            return new TemporaryGradingImagesResult
            {
                FrontImage = new TemporaryImageReference
                {
                    FileName = frontFileName,
                    FilePath = frontFilePath
                },
                BackImage = backReference
            };
        }

        public void MoveGradingImagesToFinal(
            TemporaryImageReference frontImage,
            TemporaryImageReference? backImage)
        {
            string gradingFolder = GetFolderPath("GradingImages");
            Directory.CreateDirectory(gradingFolder);

            File.Move(
                frontImage.FilePath,
                Path.Combine(gradingFolder, frontImage.FileName));

            if (backImage != null)
            {
                File.Move(
                    backImage.FilePath,
                    Path.Combine(gradingFolder, backImage.FileName));
            }
        }

        public void DeleteFilesIfExist(params string?[] filePaths)
        {
            foreach (string? path in filePaths)
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        private async Task SaveFileAsync(IFormFile image, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            await image.CopyToAsync(stream);
        }

        private string GetFolderPath(params string[] segments)
        {
            string path = _environment.ContentRootPath;

            foreach (string segment in segments)
            {
                path = Path.Combine(path, segment);
            }

            return path;
        }
    }
}