using PokeGrading.Data_input_models;

namespace PokeGrading.Utilities
{
    public class ValidationService
    {
        public static bool ValidateCardFields(
            Data_input_add_card card)
        {
            return
                !string.IsNullOrWhiteSpace(card.set_name)
                &&
                !string.IsNullOrWhiteSpace(card.card_number)
                &&
                !string.IsNullOrWhiteSpace(card.edition)
                &&
                !string.IsNullOrWhiteSpace(card.language)
                &&
                !string.IsNullOrWhiteSpace(card.finish_type)
                &&
                !string.IsNullOrWhiteSpace(card.card_name)
                &&
                !string.IsNullOrWhiteSpace(card.rarity)
                &&
                !string.IsNullOrWhiteSpace(card.pokemon_type)
                &&
                !string.IsNullOrWhiteSpace(card.illustrator);
        }
    }
}