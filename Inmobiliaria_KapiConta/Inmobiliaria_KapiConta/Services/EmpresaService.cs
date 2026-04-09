using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Data.Mappings;
using Npgsql;


namespace Inmobiliaria_KapiConta.Services
{
    public class EmpresaService
    {
        public List<Empresa> ObtenerEmpresas()
        {
            var lista = new List<Empresa>();

            using var conn = DbConnectionFactory.Create();
            conn.Open();

            using var cmd = new NpgsqlCommand(EmpresaQueries.Listar, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(EmpresaMapper.Map(reader));
            }

            return lista;
        }


    }
}
