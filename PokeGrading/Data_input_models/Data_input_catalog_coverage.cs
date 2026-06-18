namespace PokeGrading.Data_input_models
{
    public class Data_input_catalog_coverage
    {
        public string api_key { get; set; }

        public string external_request_id { get; set; }

        public List<Data_input_catalog_lookup>
            cards
        { get; set; }
    }
}