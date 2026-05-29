using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public LoginController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("login")]
        public ActionResult<Data_response<Data_output_login>>
            Login(Data_input_login input)
        {
            var user =
                _databaseService.QuerySingleOrDefault<dynamic>(
                @"SELECT *
                  FROM Users
                  WHERE Email=@Email",
                new Dictionary<string, object>
                {
                    {"Email",input.email}
                });

            if (user == null)
                return Unauthorized();

            bool valid =
                PasswordService.VerifyPassword(
                    input.password,
                    user.PasswordHash);

            if (!valid)
                return Unauthorized();

            _databaseService.ExecuteNonQuery(
                @"UPDATE Users
                  SET LastLogin = GETDATE()
                  WHERE UserId=@UserId",
                new Dictionary<string, object>
                {
                    {"UserId", user.UserId}
                });

            return Ok(new Data_response<Data_output_login>
            {
                status = true,
                data = new Data_output_login
                {
                    user_id = user.UserId,
                    email = user.Email,
                    alias = user.Alias,
                    role = user.Role
                }
            });
        }
    }
}