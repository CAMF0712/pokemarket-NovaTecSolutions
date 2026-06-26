// Contrato de servicio: define capacidades de negocio para IAuditService.
namespace PokeGrading.Services
{
    /// <summary>
    /// Contrato de auditoria para registrar eventos de negocio relevantes.
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Registra una accion realizada sobre una carta o su version.
        /// </summary>
        void LogCardAction(
            Guid userId,
            string actionType,
            Guid entityId,
            string newValue);

        /// <summary>
        /// Registra una solicitud de cobertura de catalogo para trazabilidad B2B.
        /// </summary>
        void LogApiCatalogCoverage(
            Guid apiKeyId,
            Guid requestId,
            int cardsCount);
    }
}
