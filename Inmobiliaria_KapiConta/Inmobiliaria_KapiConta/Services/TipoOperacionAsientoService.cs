using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class TipoOperacionAsientoService
    {
        public List<TipoOperacionAsiento> ObtenerTiposOperacion()
        {
            var lista = new List<TipoOperacionAsiento>();
            using var cn = DbConnectionFactory.Create();
            cn.Open();
            using var cmd = new NpgsqlCommand(TipoOperacionAsientoQueries.Listar, cn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(TipoOperacionAsientoMapper.Map(dr));
            return lista;
        }
    }
}
