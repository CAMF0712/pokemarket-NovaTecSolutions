using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;

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


        /// <summary>
        /// Realiza la autenticación de un usuario (log in).
        /// </summary>
        /// <param name="input">Objeto de tipo Data_input_log_in que contiene:
        /// - username: Nombre de usuario.
        /// - password: Contraseña del usuario.
        /// </param>
        /// <returns>
        /// Devuelve un objeto Data_response con el resultado de la autenticación.
        /// - status = true si la autenticación es exitosa.
        /// - data = objeto Data_output_log_in (por ejemplo podría incluir token de sesión, rol, etc.).
        /// </returns>
        /// <remarks>
        /// Restricciones:
        /// - El username debe existir en el sistema.
        /// - La password debe coincidir con la registrada para el username.
        /// - Aquí se asume un mock (sin lógica real de autenticación implementada).
        /// </remarks>
        [HttpPost("log_in")]
        public ActionResult<Data_response<Data_output_log_in>> LogIn([FromBody] Data_input_log_in input)
        {
            var parameters = new Dictionary<string, object>
            {
                { "in_username", input.user_name },
                { "in_password", Encriptador.ObtenerHashMD5(input.password) }
            };

            DataTable result = _databaseService.ExecuteFunction(
                "SELECT * FROM sp_log_in_user(@in_username, @in_password)", parameters);

            if (result.Rows.Count == 0)
            {
                return Unauthorized(new Data_response<Data_output_log_in>
                {
                    status = false
                });
            }

            DataRow row = result.Rows[0];

            var loginOutput = new Data_output_log_in
            {
                user_name = row["username"].ToString(),
                role = row["role"].ToString(),
                id_number = row["employee_id"].ToString()
            };

            return Ok(new Data_response<Data_output_log_in>
            {
                status = true,
                data = loginOutput
            });
        }
    }
}