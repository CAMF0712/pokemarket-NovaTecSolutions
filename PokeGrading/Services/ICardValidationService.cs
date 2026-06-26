using PokeGrading.Data_input_models;

namespace PokeGrading.Services
{
    public interface ICardValidationService
    {
        string? ValidateCreateCardInput(Data_input_add_card input);
    }
}