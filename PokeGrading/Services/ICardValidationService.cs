// Contrato de servicio: define capacidades de negocio para ICardValidationService.
using PokeGrading.Data_input_models;

namespace PokeGrading.Services
{
    /// <summary>
    /// Contrato de validaciones de entrada para operaciones de cartas.
    /// </summary>
    public interface ICardValidationService
    {
        /// <summary>
        /// Valida los datos necesarios para crear una carta y devuelve un error legible si algo falta.
        /// </summary>
        string? ValidateCreateCardInput(Data_input_add_card input);
    }
}
