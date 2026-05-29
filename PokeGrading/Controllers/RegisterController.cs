using Dapper;
using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using System.Text.RegularExpressions;
using System.Linq;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly DatabaseService _databaseService;

        public RegisterController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [HttpPost("register")]
        public ActionResult<Data_response<Data_output_register_user>>
            Register(Data_input_register_user input)
        {
            if (!input.accept_disclosure)
                return BadRequest("Disclosure must be accepted.");

            if (!Regex.IsMatch(input.email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Invalid email.");

            if (!Regex.IsMatch(input.password,
                @"^(?=.*[A-Z])(?=.*\d).{8,}$"))
                return BadRequest("Password does not meet requirements.");

            string[] validCountries =
            {
                "CR","PA","MX","CO","CL","AR"
            };

            if (!validCountries.Contains(input.country))
                return BadRequest("Country not supported.");

            var existingUser =
                _databaseService.QuerySingleOrDefault<dynamic>(
                    "SELECT * FROM Users WHERE Email=@Email",
                    new Dictionary<string, object>
                    {
                        {"Email", input.email}
                    });

            if (existingUser != null)
                return BadRequest("Email already exists.");

            Guid userId = Guid.NewGuid();

            string hash =
                PasswordService.HashPassword(input.password);

            _databaseService.ExecuteNonQuery(
                @"INSERT INTO Users
                (
                    UserId,
                    Email,
                    Alias,
                    PasswordHash,
                    Country,
                    PreferredLanguage,
                    Role,
                    Active,
                    CreatedAt,
                    LastLogin
                )
                VALUES
                (
                    @UserId,
                    @Email,
                    @Alias,
                    @PasswordHash,
                    @Country,
                    @Language,
                    'SUBMITTER',
                    1,
                    GETDATE(),
                    NULL
                )",
                new Dictionary<string, object>
                {
                    {"UserId",userId},
                    {"Email",input.email},
                    {"Alias",input.alias},
                    {"PasswordHash",hash},
                    {"Country",input.country},
                    {"Language",input.preferred_language}
                });

            return Ok(new Data_response<Data_output_register_user>
            {
                status = true,
                data = new Data_output_register_user
                {
                    user_id = userId,
                    email = input.email,
                    alias = input.alias,
                    role = "SUBMITTER",
                    created_at = DateTime.Now
                }
            });
        }
    }
}