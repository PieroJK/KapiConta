using Inmobiliaria_KapiConta.Data;
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

            string sql = @"
                SELECT id_periodo, anio
                FROM periodo
                WHERE id_empresa = @id_empresa
                ORDER BY anio DESC;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id_empresa", idEmpresa);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Periodo
                {
                    IdPeriodo = Convert.ToInt32(reader["id_periodo"]),
                    Anio = Convert.ToInt32(reader["anio"])
                });
            }

            return lista;
        }
    }
}
