namespace PokeGrading.Data_output_models
{
    public class Data_output_catalog_candidate
    {
        public Guid card_id { get; set; }

        public string set_name { get; set; }

        public string card_number { get; set; }

        public string edition { get; set; }

        public string language { get; set; }

        public string finish_type { get; set; }
    }
}