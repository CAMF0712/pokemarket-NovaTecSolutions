namespace PokeGrading.Data_output_models
{
    public class Data_output_search_card_image
    {
        public Guid card_id { get; set; }

        public string card_name { get; set; }

        public string set_name { get; set; }

        public string card_number { get; set; }

        public string image_url { get; set; }

        public string extracted_text { get; set; }
    }
}