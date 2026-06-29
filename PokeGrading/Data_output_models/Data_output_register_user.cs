namespace PokeGrading.Data_output_models
{
    public class Data_output_register_user
    {
        public Guid user_id { get; set; }

        public string email { get; set; }

        public string alias { get; set; }

        public string role { get; set; }

        public DateTime created_at { get; set; }
    }
}