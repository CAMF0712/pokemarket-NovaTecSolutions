namespace PokeGrading.Services
{
    public class StoredCardImagesResult
    {
        public required string FrontImageUrl { get; init; }

        public string? BackImageUrl { get; init; }
    }

    public class TemporaryImageReference
    {
        public required string FilePath { get; init; }

        public required string FileName { get; init; }
    }

    public class TemporaryGradingImagesResult
    {
        public required TemporaryImageReference FrontImage { get; init; }

        public TemporaryImageReference? BackImage { get; init; }
    }
}