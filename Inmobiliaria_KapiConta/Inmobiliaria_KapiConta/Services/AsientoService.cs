using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Inmobiliaria_KapiConta.Services
{
   public class AsientoService
    {

        public DataTable ObtenerAsientos(int idEmpresa, int idMes, string subDiario)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(AsientoQueries.ListarAsientos, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@subDiario", subDiario);

            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }
    }
}
