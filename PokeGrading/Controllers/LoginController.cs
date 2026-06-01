using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using PokeGrading.Models;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost("login")]
        public ActionResult<Data_response<Data_output_login>>
            Login([FromBody] Data_input_login input)
        {
            string passwordHash =
                PasswordService.HashPassword(
                    input.password);

            var user =
                FakeDatabase.Users
                    .FirstOrDefault(u =>
                        u.Email.ToLower() ==
                        input.email.ToLower());

            if (user == null)
            {
                return Unauthorized(new
                {
                    field = "email",
                    message = "User not found."
                });
            }

            if (user.PasswordHash != passwordHash)
            {
                return Unauthorized(new
                {
                    field = "password",
                    message = "Invalid password."
                });
            }

            user.LastLogin =
                DateTime.UtcNow;

            // TODO:
            // Persist LastLogin in SQL Server
            // when migrating to Dapper.

            return Ok(
                new Data_response<Data_output_login>
                {
                    status = true,
                    data =
                        new Data_output_login
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