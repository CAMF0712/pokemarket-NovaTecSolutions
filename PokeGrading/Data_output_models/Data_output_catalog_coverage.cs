namespace PokeGrading.Data_output_models
{
    public class Data_output_catalog_coverage
    {
        public Guid request_id { get; set; }

        public DateTime generated_at { get; set; }

        public List<Data_output_catalog_result>
            results
        { get; set; }
    }
}