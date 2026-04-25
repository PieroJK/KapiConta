using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class SubDiarioService
    {
        public List<SubDiario> ObtenerSubDiarios()
        {
            var lista = new List<SubDiario>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(SubDiarioQueries.Listar, cn);
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
                lista.Add(SubDiarioMapper.Map(dr));

            return lista;
        }
    }
}
