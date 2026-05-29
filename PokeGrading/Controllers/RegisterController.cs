using PokeGrading.Data_input_models;
using PokeGrading.Data_output_models;
using PokeGrading.Utilities;
using Microsoft.AspNetCore.Mvc;

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


        /// <summary>
        /// Registra un nuevo cliente en el sistema.
        /// </summary>
        /// <param name="input">Objeto Data_input_register_client con los datos del cliente a registrar.</param>
        /// <returns>
        /// Data_response con los datos del cliente registrado.
        /// - status: true si el registro fue exitoso.
        /// - data: Data_output_register_client con la información del cliente registrado.
        /// </returns>
        /// <remarks>
        /// Se espera que el input contenga toda la información necesaria para el registro.
        /// </remarks>
        [HttpPost("register_client")]
        public ActionResult<Data_response<Data_output_register_client>> RegisterClient([FromBody] Data_input_register_client input)
        {
            var parameters = new Dictionary<string, object>
            {
                { "in_client_id", input.id_number },
                { "in_first_name", input.first_name },
                { "in_user_name", input.user_name },
                { "in_last_name_1", input.last_name_1 },
                { "in_last_name_2", input.last_name_2 },
                { "in_birth_date", input.birth_date },
                { "in_weight", input.weight },
                { "in_address", input.address },
                { "in_email", input.email },
                { "in_password", Encriptador.ObtenerHashMD5(input.password) },
                { "in_phone", input.phone }
            };

            try
            {
                _databaseService.ExecuteFunction("SELECT sp_register_client(@in_client_id, @in_first_name, @in_user_name, @in_last_name_1, @in_last_name_2, @in_birth_date, @in_weight, @in_address, @in_email, @in_password, @in_phone)", parameters);

                var data_Output = new Data_output_register_client
                {
                    id_number = input.id_number.ToString(),
                    first_name = input.first_name,
                    user_name = input.user_name,
                    last_name_1 = input.last_name_1,
                    last_name_2 = input.last_name_2,
                    birth_date = input.birth_date,
                    weight = input.weight,
                    imc = input.imc, // Este se sigue enviando aunque no se almacene
                    address = input.address,
                    email = input.email,
                    role = input.role,
                    phone = input.phone
                };

                return Ok(new Data_response<Data_output_register_client>
                {
                    status = true,
                    data = data_Output
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = false,
                    error = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}