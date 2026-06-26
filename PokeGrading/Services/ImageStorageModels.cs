// Modelos de apoyo para resultados de almacenamiento de imagenes.
namespace PokeGrading.Services
{
    /// <summary>
    /// Resultado de guardado final de imagenes de una carta.
    /// </summary>
    public class StoredCardImagesResult
    {
        /// <summary>
        /// Ruta publica de la imagen frontal almacenada.
        /// </summary>
        public required string FrontImageUrl { get; init; }

        /// <summary>
        /// Ruta publica de la imagen trasera cuando existe.
        /// </summary>
        public string? BackImageUrl { get; init; }
    }

    /// <summary>
    /// Referencia a un archivo temporal creado durante un flujo de carga.
    /// </summary>
    public class TemporaryImageReference
    {
        /// <summary>
        /// Ruta absoluta del archivo temporal en disco.
        /// </summary>
        public required string FilePath { get; init; }

        /// <summary>
        /// Nombre del archivo temporal.
        /// </summary>
        public required string FileName { get; init; }
    }

    /// <summary>
    /// Resultado temporal de imagenes para una solicitud de grading.
    /// </summary>
    public class TemporaryGradingImagesResult
    {
        /// <summary>
        /// Imagen frontal temporal obligatoria.
        /// </summary>
        public required TemporaryImageReference FrontImage { get; init; }

        /// <summary>
        /// Imagen trasera temporal cuando fue enviada.
        /// </summary>
        public TemporaryImageReference? BackImage { get; init; }
    }
}
