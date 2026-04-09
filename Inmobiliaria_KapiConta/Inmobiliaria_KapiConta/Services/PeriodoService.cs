using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class PeriodoService
    {
        public List<Periodo> ObtenerPeriodos(int idEmpresa)
        {
            var lista = new List<Periodo>();

            using var conn = DbConnectionFactory.Create();
            conn.Open();

            // 🔥 Usamos el Query centralizado
            using var cmd = new NpgsqlCommand(PeriodoQueries.Listar, conn);
            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // 🔥 Usamos el Mapper
                lista.Add(PeriodoMapper.Map(reader));
            }

            return lista;
        }
    }
}