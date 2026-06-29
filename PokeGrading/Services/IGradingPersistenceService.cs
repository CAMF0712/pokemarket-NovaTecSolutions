// Contrato de servicio: define capacidades de negocio para IGradingPersistenceService.
namespace PokeGrading.Services
{
    /// <summary>
    /// Datos consolidados que se persisten al registrar un grading.
    /// </summary>
    public class GradingPersistenceRequest
    {
        /// <summary>
        /// Usuario que realiza la solicitud de grading.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Carta evaluada en el proceso de grading.
        /// </summary>
        public required Guid CardId { get; init; }

        /// <summary>
        /// Nota final estimada para la carta.
        /// </summary>
        public required decimal EstimatedGrade { get; init; }

        /// <summary>
        /// Nivel de confianza calculado para la nota.
        /// </summary>
        public required decimal ConfidenceScore { get; init; }

        /// <summary>
        /// Estado del flujo de grading.
        /// </summary>
        public required string Status { get; init; }

        /// <summary>
        /// Subscore de centrado.
        /// </summary>
        public required decimal Centering { get; init; }

        /// <summary>
        /// Subscore de esquinas.
        /// </summary>
        public required decimal Corners { get; init; }

        /// <summary>
        /// Subscore de bordes.
        /// </summary>
        public required decimal Edges { get; init; }

        /// <summary>
        /// Subscore de superficie.
        /// </summary>
        public required decimal Surface { get; init; }

        /// <summary>
        /// URL final de la imagen frontal.
        /// </summary>
        public required string FrontImageUrl { get; init; }

        /// <summary>
        /// URL final de la imagen trasera cuando existe.
        /// </summary>
        public string? BackImageUrl { get; init; }
    }

    /// <summary>
    /// Contrato para persistencia transaccional de grading.
    /// </summary>
    public interface IGradingPersistenceService
    {
        /// <summary>
        /// Guarda un grading completo y devuelve su identificador generado.
        /// </summary>
        Guid SaveGrading(GradingPersistenceRequest request);
    }
}

