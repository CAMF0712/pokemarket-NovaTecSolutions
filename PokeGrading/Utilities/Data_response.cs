namespace PokeGrading.Utilities
{
    public class Data_response<T>
    {
        // Estado de la respuesta
        public bool status { get; set; }

        // Datos devueltos
        public T data { get; set; }
    }
}