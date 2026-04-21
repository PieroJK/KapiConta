using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class TipoCambioMapper
    {
        public static TipoCambio Map(NpgsqlDataReader dr)
        {
            return new TipoCambio
            {
                IdTipoCambio = Convert.ToInt32(dr["id_tipo_cambio"]),
                Fecha = Convert.ToDateTime(dr["fecha"]),
                Moneda = dr["moneda"]?.ToString() ?? string.Empty,
                Compra = dr["compra"] != DBNull.Value ? Convert.ToDecimal(dr["compra"]) : null,
                Venta = dr["venta"] != DBNull.Value ? Convert.ToDecimal(dr["venta"]) : null,
                Estado = Convert.ToBoolean(dr["estado"])
            };
        }
    }
}
