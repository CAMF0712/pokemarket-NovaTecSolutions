// Controlador HTTP: coordina el flujo de entrada/salida para LoginController.
using Microsoft.AspNetCore.Mvc;
using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Models;
using PokeGrading.Repositories;
using PokeGrading.Utilities;

namespace PokeGrading.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /// <summary>
    /// Clase principal que concentra la responsabilidad de LoginController en esta capa.
    /// </summary>
    public class LoginController : ControllerBase
    {
        private const int AdminRoleId = 3;
        private const int ModeratorRoleId = 2;

        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Inicializa una nueva instancia de LoginController.
        /// </summary>
        public LoginController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public ActionResult<Data_response<Data_output_login>>
        Login([FromBody] Data_input_login input)
        {
            this.EnsureTraceId();

            User? user = _userRepository.GetByEmail(input.email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    field = "email",
                    message = "User not found."
                });
            }

            if (!PasswordService.VerifyPassword(input.password, user.PasswordHash))
            {
                return Unauthorized(new
                {
                    field = "password",
                    message = "Invalid password."
                });
            }

            _userRepository.UpdateLastLogin(user.UserId);

            string roleName =
                user.RoleId switch
                {
                    AdminRoleId => "ADMIN",
                    ModeratorRoleId => "MODERATOR",
                    _ => "SUBMITTER"
                };

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


