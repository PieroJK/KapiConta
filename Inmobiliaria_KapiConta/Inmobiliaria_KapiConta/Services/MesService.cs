using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class MesService
    {
        public List<Mes> ObtenerMeses()
        {
            var lista = new List<Mes>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(MesQueries.Listar, cn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(MesMapper.Map(dr));

            return lista;
        }
    }
}