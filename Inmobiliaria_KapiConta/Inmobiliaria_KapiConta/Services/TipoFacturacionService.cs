using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class TipoFacturacionService
    {
        public List<TipoFacturacion> ObtenerTiposFacturacion()
        {
            var lista = new List<TipoFacturacion>();
            using var cn = DbConnectionFactory.Create();
            cn.Open();
            using var cmd = new NpgsqlCommand(TipoFacturacionQueries.Listar, cn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(TipoFacturacionMapper.Map(dr));
            return lista;
        }
    }
}
