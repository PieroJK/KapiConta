using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class CostoMapper
    {
        public static Costo Map(NpgsqlDataReader dr)
        {
            var costo = new Costo
            {
                IdCosto = Convert.ToInt32(dr["id_costo"]),
                IdEmpresa = Convert.ToInt32(dr["id_empresa"]),
                Codigo = dr["codigo"]?.ToString() ?? string.Empty,
                Descripcion = dr["descripcion"]?.ToString() ?? string.Empty,
                Estado = Convert.ToBoolean(dr["estado"])
            };

            if (dr.HasColumn("empresa_nombre"))
                costo.Empresa = new Empresa
                {
                    IdEmpresa = costo.IdEmpresa,
                    Nombre = dr["empresa_nombre"]?.ToString() ?? string.Empty
                };

            return costo;
        }
    }
}
