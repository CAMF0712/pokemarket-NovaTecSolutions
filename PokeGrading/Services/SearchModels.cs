// Modelos usados por OCR y scoring para busqueda de cartas.
namespace PokeGrading.Services
{
    /// <summary>
    /// Atributos detectados automaticamente desde el texto OCR.
    /// </summary>
    public class CardAttributesFromOcr
    {
        /// <summary>
        /// Obtiene o establece el valor de DetectedHp.
        /// </summary>
        public int? DetectedHp { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de DetectedCardNumber.
        /// </summary>
        public string? DetectedCardNumber { get; set; }

        /// <summary>
        /// Obtiene o establece el valor de DetectedType.
        /// </summary>
        public string? DetectedType { get; set; }
    }

    /// <summary>
    /// Representa un candidato de busqueda junto con su puntaje.
    /// </summary>
    public class ScoredCandidate
    {
        /// <summary>
        /// Puntaje total calculado para el candidato.
        /// </summary>
        public required int Score { get; init; }

        /// <summary>
        /// Datos de carta devueltos desde persistencia.
        /// </summary>
        public required dynamic Card { get; init; }
    }

    /// <summary>
    /// Resultado final del proceso de busqueda por imagen.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Candidatos ordenados por relevancia.
        /// </summary>
        public required List<ScoredCandidate> ScoredCandidates { get; init; }

        /// <summary>
        /// Mejor coincidencia cuando supera el umbral requerido.
        /// </summary>
        public required ScoredCandidate? BestMatch { get; init; }

        /// <summary>
        /// Porcentaje de confianza calculado para la seleccion.
        /// </summary>
        public required double Confidence { get; init; }
    }
}

