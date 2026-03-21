using Npgsql;

namespace Inmobiliaria_KapiConta.Data
{
    public class DbConnectionFactory
    {
        public static NpgsqlConnection Create()
        {
            string host = "localhost";
            string port = "5432";
            string database = "db_kapiconta";
            string user = "postgres";

            // 🔐 contraseña desde variable de entorno
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            string connectionString =
                $"Host={host};Port={port};Database={database};Username={user};Password={password}";

            return new NpgsqlConnection(connectionString);
        }
    }
}