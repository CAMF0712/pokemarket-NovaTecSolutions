namespace PokeGrading.Data_output_models
{
    public class Data_output_submit_grading
    {
        public Guid grading_id { get; set; }

        public Guid card_id { get; set; }

        public decimal estimated_grade { get; set; }

        public decimal confidence_score { get; set; }

        public string status { get; set; }
    }
}