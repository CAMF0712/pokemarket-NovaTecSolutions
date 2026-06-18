using Dapper;
using Microsoft.Data.SqlClient;

namespace PokeGrading.Utilities
{
    /// <summary>
    /// Servicio de acceso a datos que encapsula la apertura de conexiones
    /// y la ejecución de queries y comandos SQL mediante Dapper.
    /// </summary>
    public class DatabaseService
    {
        private readonly SQL_connection _sqlConnection;

        public DatabaseService(
            SQL_connection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        /// <summary>
        /// Ejecuta un conjunto de operaciones SQL dentro de una única transacción.
        /// Si cualquier operación falla se hace rollback automático y se relanza la excepción.
        /// </summary>
        /// <param name="work">Acción que recibe la conexión abierta y la transacción activa.</param>
        public void ExecuteInTransaction(
            Action<SqlConnection, SqlTransaction> work)
        {
            using var conn = _sqlConnection.GetConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                work(conn, tx);
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public T QuerySingle<T>(
            string sql,
            Dictionary<string, object> parameters)
        {
            using var conn =
                _sqlConnection.GetConnection();

            conn.Open();

            return conn.QuerySingle<T>(
                sql,
                new DynamicParameters(parameters));
        }

        public T QuerySingleOrDefault<T>(
            string sql,
            Dictionary<string, object> parameters)
        {
            using var conn =
                _sqlConnection.GetConnection();

            conn.Open();

            return conn.QuerySingleOrDefault<T>(
                sql,
                new DynamicParameters(parameters));
        }

        public IEnumerable<T> QueryList<T>(
            string sql,
            Dictionary<string, object> parameters)
        {
            using var conn =
                _sqlConnection.GetConnection();

            conn.Open();

            return conn.Query<T>(
                sql,
                new DynamicParameters(parameters));
        }

        public IEnumerable<dynamic> Query(
            string sql,
            Dictionary<string, object> parameters)
        {
            using var conn =
                _sqlConnection.GetConnection();

            conn.Open();

            return conn.Query(
                sql,
                new DynamicParameters(parameters));
        }

        public int ExecuteNonQuery(
            string sql,
            Dictionary<string, object> parameters)
        {
            using var conn =
                _sqlConnection.GetConnection();

            conn.Open();

            return conn.Execute(
                sql,
                new DynamicParameters(parameters));
        }
    }
}