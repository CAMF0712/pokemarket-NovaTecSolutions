namespace PokeGrading.Data_output_models
{
    public class Data_output_catalog_result
    {
        public string status { get; set; }

        public string message { get; set; }

        public Guid? card_id { get; set; }

        public string set_name { get; set; }

        public string card_number { get; set; }

        public string edition { get; set; }

        public string language { get; set; }

        public string finish_type { get; set; }

        public List<Data_output_catalog_candidate>
            candidates
        { get; set; }
    }
}