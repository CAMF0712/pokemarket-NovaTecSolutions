using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Repositories;
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

        private readonly IUserRepository _userRepository;

        public RegisterController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public ActionResult<Data_response<Data_output_register_user>>
            Register([FromBody] Data_input_register_user input)
        {
            this.EnsureTraceId();

            if (!input.accept_disclosure)
            {
                return BadRequest(new
                {
                    field = "accept_disclosure",
                    message = "Disclosure must be accepted."
                });
            }

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

            if (_userRepository.EmailExists(input.email))
            {
                return BadRequest(new
                {
                    field = "email",
                    message = "Email already exists."
                });
            }

            Guid userId = Guid.NewGuid();

            string passwordHash =
                PasswordService.HashPassword(
                    input.password);

            string language =
                input.preferred_language
                     .ToUpper();

            _userRepository.CreateUser(
                new UserRegistrationRecord
                {
                    UserId = userId,
                    RoleId = DefaultUserRoleId,
                    Email = input.email,
                    Alias = input.alias,
                    PasswordHash = passwordHash,
                    Country = input.country,
                    PreferredLanguage = language,
                    Status = "ACTIVE",
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                });

            return Ok(
                new Data_response<Data_output_register_user>
                {
                    status = true,

                    data =
                        new Data_output_register_user
                        {
                            user_id = userId,
                            email = input.email,
                            alias = input.alias,
                            role = "USER",
                            created_at = DateTime.UtcNow
                        }
                });
        }
    }
}

