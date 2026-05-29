namespace PokeGrading.Data_input_models
{
    public class Data_input_register_user
    {
        public string email { get; set; }

        public string alias { get; set; }

        public string password { get; set; }

        public string country { get; set; }

        public string preferred_language { get; set; }

        public bool accept_disclosure { get; set; }
    }
}