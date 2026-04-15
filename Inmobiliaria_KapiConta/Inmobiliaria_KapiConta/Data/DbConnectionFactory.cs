using Npgsql;

namespace Inmobiliaria_KapiConta.Data
{
    public static class DbConnectionFactory
    {
        private static readonly string connectionString;

        static DbConnectionFactory()
        {
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (string.IsNullOrEmpty(password))
                throw new Exception("DB_PASSWORD no definida");

            connectionString =
                $"Host=localhost;Port=5432;Database=prueba;Username=postgres;Password={password}";
        }

        public static NpgsqlConnection Create()
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}