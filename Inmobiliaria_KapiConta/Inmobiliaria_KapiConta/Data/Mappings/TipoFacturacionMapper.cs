using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class TipoFacturacionMapper
    {
        public static TipoFacturacion Map(NpgsqlDataReader dr)
        {
            return new TipoFacturacion
            {
                IdTipoFacturacion = Convert.ToInt32(dr["id_tipo_facturacion"]),
                Cod = dr["cod"]?.ToString() ?? string.Empty,
                Nombre = dr["nombre"]?.ToString() ?? string.Empty,
                Estado = Convert.ToBoolean(dr["estado"])
            };
        }
    }
}
