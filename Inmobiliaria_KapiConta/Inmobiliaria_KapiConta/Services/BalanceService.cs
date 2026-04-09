using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class BalanceService
    {
        public List<Balance> ObtenerBalances()
        {
            var lista = new List<Balance>();

            using var conn = DbConnectionFactory.Create();
            conn.Open();

            using var cmd = new NpgsqlCommand(BalanceQueries.Listar, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(BalanceMapper.Map(reader));
            }

            return lista;
        }
    }
}
