using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using System.Text.RegularExpressions;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
        private const int DefaultUserRoleId = 1;
        private const int MinimumPasswordLength = 8;

        private const string PasswordPolicyPattern =
            @"^(?=.*[A-Z])(?=.*\d).{8,}$";

        private readonly DatabaseService _database;

        public RegisterController(DatabaseService database)
        {
            _database = database;
        }

        [HttpPost("register")]
        public ActionResult<Data_response<Data_output_register_user>>
            Register([FromBody] Data_input_register_user input)
        {
            //-----------------------------------
            // Disclosure
            //-----------------------------------

            if (!input.accept_disclosure)
            {
                return BadRequest(new
                {
                    field = "accept_disclosure",
                    message = "Disclosure must be accepted."
                });
            }

            //-----------------------------------
            // Email format
            //-----------------------------------

            if (!Regex.IsMatch(
                input.email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                return BadRequest(new
                {
                    field = "email",
                    message = "Invalid email format."
                });
            }

            //-----------------------------------
            // Disposable domains
            //-----------------------------------

            string[] blockedDomains =
            {
                "mailinator.com",
                "yopmail.com",
                "tempmail.com",
                "10minutemail.com"
            };

            string domain =
                input.email
                     .Split('@')
                     .Last()
                     .ToLower();

            if (blockedDomains.Contains(domain))
            {
                return BadRequest(new
                {
                    field = "email",
                    message = "Disposable email domains are not allowed."
                });
            }

            //-----------------------------------
            // Password validation
            //-----------------------------------

            if (!Regex.IsMatch(
                input.password,
                PasswordPolicyPattern))
            {
                return BadRequest(new
                {
                    field = "password",
                    message = $"Password must contain at least one uppercase letter, one number and be at least {MinimumPasswordLength} characters long."
                });
            }

            //-----------------------------------
            // Country validation
            //-----------------------------------

            string[] validCountries =
            {
                "CR",
                "PA",
                "MX",
                "CO",
                "CL",
                "AR"
            };

            if (!validCountries.Contains(input.country))
            {
                return BadRequest(new
                {
                    field = "country",
                    message = "Country not supported."
                });
            }

            //-----------------------------------
            // Unique email validation
            //-----------------------------------

            var existingUser =
                _database.QuerySingleOrDefault<Guid?>(
                    @"SELECT user_id
                      FROM USERS
                      WHERE LOWER(email) = LOWER(@email)",
                    new Dictionary<string, object>
                    {
                        { "email", input.email }
                    });

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    field = "email",
                    message = "Email already exists."
                });
            }

            //-----------------------------------
            // Create user
            //-----------------------------------

            Guid userId = Guid.NewGuid();

            string passwordHash =
                PasswordService.HashPassword(
                    input.password);

            string language =
                input.preferred_language
                     .ToUpper();

            string sql =
            @"
                INSERT INTO USERS
                (
                    user_id,
                    role_id,
                    email,
                    alias,
                    password_hash,
                    country,
                    preferred_language,
                    status,
                    active,
                    created_at
                )
                VALUES
                (
                    @user_id,
                    @role_id,
                    @email,
                    @alias,
                    @password_hash,
                    @country,
                    @preferred_language,
                    @status,
                    @active,
                    @created_at
                )
            ";

            _database.ExecuteNonQuery(
                sql,
                new Dictionary<string, object>
                {
                    { "user_id", userId },
                    { "role_id", DefaultUserRoleId }, // USER
                    { "email", input.email },
                    { "alias", input.alias },
                    { "password_hash", passwordHash },
                    { "country", input.country },
                    { "preferred_language", language },
                    { "status", "ACTIVE" },
                    { "active", true },
                    { "created_at", DateTime.UtcNow }
                });

            //-----------------------------------
            // Response
            //-----------------------------------

            return Ok(
                new Data_response<Data_output_register_user>
                {
                    status = true,

                    data =
                        new Data_output_register_user
                        {
                            user_id = userId,

                            email =
                                input.email,

                            alias =
                                input.alias,

                            role =
                                "USER",

                            created_at =
                                DateTime.UtcNow
                        }
                });
        }
    }
}