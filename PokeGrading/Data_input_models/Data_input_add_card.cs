namespace PokeGrading.Data_input_models
{
    public class Data_input_add_card
    {
        public string set_name { get; set; }

        public string card_number { get; set; }

        public string edition { get; set; }

        public string language { get; set; }

        public string finish_type { get; set; }

        public string card_name { get; set; }

        public string rarity { get; set; }

        public string pokemon_type { get; set; }

        public int hp { get; set; }

        public string illustrator { get; set; }

        public int release_year { get; set; }

        public string image_url { get; set; }

        public Guid created_by { get; set; }
    }
}