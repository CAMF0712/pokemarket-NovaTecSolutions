using Microsoft.Data.SqlClient;

namespace PokeGrading.Utilities
{
    public class SQL_connection
    {
        private readonly string _connectionString;

        public SQL_connection(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString(
                    "DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(
                _connectionString);
        }
    }
}