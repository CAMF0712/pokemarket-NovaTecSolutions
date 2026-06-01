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
        private readonly DatabaseService _databaseService;

        public LoginController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("login")]
        public ActionResult<Data_response<Data_output_login>>
        Login([FromBody] Data_input_login input)
        {
            string passwordHash =
                PasswordService.HashPassword(
                    input.password);

            var user =
                _databaseService.QuerySingleOrDefault<User>(
                @"
                SELECT
                    user_id      AS UserId,
                    role_id      AS RoleId,
                    email        AS Email,
                    alias        AS Alias,
                    password_hash AS PasswordHash,
                    country      AS Country,
                    preferred_language AS PreferredLanguage,
                    status       AS Status,
                    active       AS Active,
                    created_at   AS CreatedAt,
                    last_login   AS LastLogin
                FROM USERS
                WHERE email = @Email
                ",
                new Dictionary<string, object>
                {
                    { "Email", input.email }
                });

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

            //-----------------------------------
            // Update Last Login
            //-----------------------------------

            _databaseService.ExecuteNonQuery(
                @"
                UPDATE USERS
                SET last_login = GETUTCDATE()
                WHERE user_id = @UserId
                ",
                new Dictionary<string, object>
                {
                    { "UserId", user.UserId }
                });

            //-----------------------------------
            // Role Mapping
            //-----------------------------------

            string roleName =
                user.RoleId switch
                {
                    3 => "ADMIN",
                    2 => "MODERATOR",
                    _ => "SUBMITTER"
                };

            //-----------------------------------
            // Response
            //-----------------------------------

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
                            role = roleName
                        }
                });
        }
    }
}