namespace PokeGrading.Data_output_models
{
    public class Data_output_log_in
    {
        // Usuario autenticado
        public string user_name { get; set; }

        // Rol asignado al usuario (Admin, Cliente, Instructor)
        public string role { get; set; }

        // Número de cédula del usuario
        public string id_number { get; set; }
    }
}
