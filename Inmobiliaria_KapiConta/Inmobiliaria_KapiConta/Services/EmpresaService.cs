using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Models;
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

            string sql = @"SELECT id_empresa, nombre FROM empresa ORDER BY nombre;";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Empresa
                {
                    IdEmpresa = Convert.ToInt32(reader["id_empresa"]),
                    Nombre = reader["nombre"]?.ToString() ?? ""
                });
            }

            return lista;
        }
    }
}
