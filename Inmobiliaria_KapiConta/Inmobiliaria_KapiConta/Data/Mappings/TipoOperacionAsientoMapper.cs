using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class TipoOperacionAsientoMapper
    {
        public static TipoOperacionAsiento Map(NpgsqlDataReader dr)
        {
            return new TipoOperacionAsiento
            {
                IdTipoOperacion = Convert.ToInt32(dr["id_tipo_operacion"]),
                Codigo = dr["codigo"]?.ToString() ?? string.Empty,
                Nombre = dr["nombre"]?.ToString() ?? string.Empty,
                Estado = Convert.ToBoolean(dr["estado"])
            };
        }
    }
}
