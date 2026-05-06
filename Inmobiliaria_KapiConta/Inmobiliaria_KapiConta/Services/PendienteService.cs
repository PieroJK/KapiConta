using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models.DTOs;
using Npgsql;
using System.Collections.Generic;

namespace Inmobiliaria_KapiConta.Services
{
    public class PendienteService
    {
        public List<PendienteItem> ObtenerAnalisisPendientes(int idEmpresa)
        {
            var lista = new List<PendienteItem>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(PendienteQueries.ObtenerPendientes, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(PendienteItemMapper.Map(dr));
            }

            return lista;
        }
    }
}
