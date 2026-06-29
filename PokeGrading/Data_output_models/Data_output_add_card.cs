namespace PokeGrading.Data_output_models
{
    public class Data_output_add_card
    {
        public Guid card_id { get; set; }

        public Guid version_id { get; set; }

        public string card_name { get; set; }
    }
}