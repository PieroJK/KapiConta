using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class ElementoService
    {
        public List<Elemento> ObtenerElementos()
        {
            var lista = new List<Elemento>();

            using var conn = DbConnectionFactory.Create();
            conn.Open();

            using var cmd = new NpgsqlCommand(ElementoQueries.Listar, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(ElementoMapper.Map(reader));
            }

            return lista;
        }
    }
}