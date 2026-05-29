namespace PokeGrading.Data_input_models
{
    public class Data_input_register_client
    {
        public int id_number { get; set; }
        public string first_name { get; set; }
        public string user_name { get; set; }
        public string last_name_1 { get; set; }
        public string last_name_2 { get; set; }
        public int age { get; set; }
        public string birth_date { get; set; } // Formato: DD/MM/YYYY HH:MM
        public int weight { get; set; }
        public double imc { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; } // Siempre será "Cliente"
        public string phone { get; set; }
    }
}