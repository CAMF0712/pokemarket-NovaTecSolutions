using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;

namespace PokeGrading.Services.Application
{
    public interface IGradingApplicationService
    {
        Task<ActionResult<Data_response<Data_output_submit_grading>>>
            SubmitGrading(
                Data_input_submit_grading input);
    }
}