using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Data.Mappings;
using Npgsql;
using Dapper;


namespace Inmobiliaria_KapiConta.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        public List<Empresa> ListEnterprise()
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

        public void AddEnterprise(Empresa e)
        {
            using var conn = DbConnectionFactory.Create();
            var id = conn.QuerySingle<int>(EmpresaQueries.Insertar, new
            {
                ruc = e.Ruc,
                nombre = e.Nombre,
                direccion = e.Direccion,
                estado = true
            });

            e.IdEmpresa = id;
        }

        public void UpdateEnterprise(Empresa e)
        {
            using var conn = DbConnectionFactory.Create();
            conn.Execute(EmpresaQueries.Actualizar, e);
        }

    }
}
