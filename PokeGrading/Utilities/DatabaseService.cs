
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
namespace PokeGrading.Utilities
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = "Host=gymtec.postgres.database.azure.com;Port=5432;Database=GymTEC;Username=Darga1905;Password=Dagamo1905.;Trust Server Certificate=true;Include Error Detail=true";
        }

        /// Ejecuta un procedimiento almacenado de PostgreSQL y devuelve un DataTable.
        public DataTable ExecuteFunction(string functionCall, Dictionary<string, object> parameters)
        {
            var dataTable = new DataTable();

            using var connection = new NpgsqlConnection(_connectionString);
            using var command = new NpgsqlCommand(functionCall, connection)
            {
                CommandType = CommandType.Text
            };

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            using var adapter = new NpgsqlDataAdapter(command);
            adapter.Fill(dataTable);

            return dataTable;
        }

        public T QuerySingle<T>(string sql, Dictionary<string, object> parameters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var dynamicParams = new DynamicParameters();
            foreach (var param in parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            return conn.QuerySingle<T>(sql, dynamicParams);
        }

        public T QuerySingleOrDefault<T>(string sql, Dictionary<string, object> parameters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var dynamicParams = new DynamicParameters();
            foreach (var param in parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            return conn.QuerySingleOrDefault<T>(sql, dynamicParams);
        }
        public IEnumerable<T> QueryList<T>(string sql, Dictionary<string, object> parameters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();

            var dynamicParams = new DynamicParameters();
            foreach (var param in parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            return conn.Query<T>(sql, dynamicParams);
        }

        public IEnumerable<dynamic> Query(string sql, Dictionary<string, object> parameters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn.Query(sql, new DynamicParameters(parameters));
        }

        public IEnumerable<T> Query2<T>(string sql, Dictionary<string, object> parameters)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn.Query<T>(sql, new DynamicParameters(parameters));
        }

    }
}
