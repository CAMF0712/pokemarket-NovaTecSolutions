namespace PokeGrading.Data_output_models
{
    public class Data_output_add_card
    {
        public Guid card_id { get; set; }

        public string card_name { get; set; }

        public string set_name { get; set; }

        public string card_number { get; set; }

        public DateTime created_at { get; set; }
    }
}