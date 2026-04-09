using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class BalanceMapper
    {
        public static Balance Map(NpgsqlDataReader reader)
        {
            return new Balance
            {
                IdBalance = (int)reader["id_balance"],
                Nombre = reader["nombre"]?.ToString() ?? string.Empty
            };
        }
    }
}
