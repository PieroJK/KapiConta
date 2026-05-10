using Inmobiliaria_KapiConta.Models.DTOs;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class AsientoBusquedaRegistroItemMapper
    {
        public static AsientoBusquedaRegistroItem Map(NpgsqlDataReader dr)
        {
            return new AsientoBusquedaRegistroItem
            {
                IdAsiento = Convert.ToInt32(dr["id_asiento"]),

                Referencia = dr["referencia"]?.ToString() ?? string.Empty,

                MesCodigo = dr["mes_codigo"]?.ToString() ?? string.Empty,

                SubDiarioCodigo = dr["subdiario_codigo"]?.ToString() ?? string.Empty,

                FechaTexto = dr["fecha_texto"]?.ToString() ?? string.Empty,

                Glosa = dr["glosa"]?.ToString() ?? string.Empty,

                Importe = dr["importe"] != DBNull.Value
                    ? Convert.ToDecimal(dr["importe"])
                    : 0,

                Usuario = dr["usuario"]?.ToString() ?? string.Empty
            };
        }
    }
}
