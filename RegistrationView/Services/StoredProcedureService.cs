using Microsoft.Data.SqlClient;
using RegistrationView.Models;
using System.Data;

namespace RegistrationView.Services
{
    public class StoredProcedureService
    {
        private readonly string _connectionString;

        public StoredProcedureService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<UserModel> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[] parameters)
        {
            var users = new UserModel();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddRange(parameters);

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                return new UserModel
                {
                    ID = reader.GetInt64(reader.GetOrdinal("ID")), // Use GetInt32 if SQL column is INT
                    Fullname = reader.GetString(reader.GetOrdinal("Fullname")),
                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                    Passwords = reader.GetString(reader.GetOrdinal("Passwords"))
                };
            }
            return users;
        }
    }
}

