namespace PokeGrading.Data_output_models
{
    public class Data_output_api_error
    {
        public string error_code { get; set; }

        public string message { get; set; }

        public string trace_id { get; set; }
    }
}