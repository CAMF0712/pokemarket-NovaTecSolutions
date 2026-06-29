using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Services.Application;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GradingController : ControllerBase
    {
        private readonly IGradingApplicationService
            _gradingApplicationService;

        public GradingController(
            IGradingApplicationService gradingApplicationService)
        {
            _gradingApplicationService =
                gradingApplicationService;
        }

        [HttpPost("submit")]
        public async Task<ActionResult<Data_response<Data_output_submit_grading>>>
            SubmitGrading(
                [FromForm]
                Data_input_submit_grading input)
        {
            this.EnsureTraceId();

            return await
                _gradingApplicationService
                    .SubmitGrading(input);
        }
    }
}