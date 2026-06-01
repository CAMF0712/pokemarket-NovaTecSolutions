using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using PokeGrading.Models;
using System.Text.RegularExpressions;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegisterController : ControllerBase
    {
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
                @"^(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                return BadRequest(new
                {
                    field = "password",
                    message = "Password must contain at least one uppercase letter, one number and be at least 8 characters long."
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
                FakeDatabase.Users
                    .FirstOrDefault(
                        u =>
                        u.Email.ToLower() ==
                        input.email.ToLower());

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

            var newUser =
                new User
                {
                    UserId = userId,
                    Email = input.email,
                    Alias = input.alias,
                    PasswordHash = passwordHash,
                    Country = input.country,
                    PreferredLanguage =
                        input.preferred_language,

                    Role = "SUBMITTER",

                    Active = true,

                    CreatedAt =
                        DateTime.UtcNow,

                    LastLogin = null
                };

            FakeDatabase.Users.Add(newUser);

            //-----------------------------------
            // TODO Database
            //-----------------------------------

            // TODO:
            // Replace FakeDatabase.Users.Add(...)
            // with SQL Server persistence
            // through Dapper.

            //-----------------------------------
            // Response
            //-----------------------------------

            return Ok(
                new Data_response
                <
                    Data_output_register_user
                >
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
                                "SUBMITTER",

                            created_at =
                                DateTime.UtcNow
                        }
                });
        }
    }
}