using Dapper;

namespace PokeGrading.Utilities
{
    public class DatabaseService
    {
        private readonly SQL_connection _sqlConnection;

        public DatabaseService(
            SQL_connection sqlConnection)
        {
            _sqlConnection = sqlConnection;
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