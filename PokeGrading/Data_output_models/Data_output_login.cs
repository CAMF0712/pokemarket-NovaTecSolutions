namespace PokeGrading.Data_output_models
{
    public class Data_output_login
    {
        public Guid user_id { get; set; }

        public string email { get; set; }

        public string alias { get; set; }

        public string role { get; set; }
    }
}