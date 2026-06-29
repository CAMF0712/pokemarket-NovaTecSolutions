using Microsoft.AspNetCore.Http;

namespace PokeGrading.Data_input_models
{
    public class Data_input_submit_grading
    {
        public Guid user_id { get; set; }

        public Guid card_id { get; set; }

        public IFormFile front_image { get; set; }

        public IFormFile? back_image { get; set; }
    }
}