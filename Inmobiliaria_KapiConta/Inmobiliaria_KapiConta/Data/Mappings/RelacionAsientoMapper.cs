using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class RelacionAsientoMapper
    {
        public static RelacionAsiento Map(NpgsqlDataReader dr)
        {
            var relacion = new RelacionAsiento
            {
                IdRelacion = Convert.ToInt32(dr["id_relacion"]),
                AsientoOrigen = Convert.ToInt32(dr["asiento_origen"]),
                AsientoRelacionado = Convert.ToInt32(dr["asiento_relacionado"]),
                Estado = Convert.ToBoolean(dr["estado"])
            };

            // Asiento origen
            if (dr.HasColumn("origen_fecha"))
                relacion.Origen = new Asiento
                {
                    IdAsiento = relacion.AsientoOrigen,
                    Fecha = Convert.ToDateTime(dr["origen_fecha"]),
                    Referencia = dr["origen_referencia"] != DBNull.Value
                                    ? dr["origen_referencia"].ToString() : null,
                    Moneda = dr["origen_moneda"]?.ToString() ?? "PEN"
                };

            // Asiento relacionado
            if (dr.HasColumn("relacionado_fecha"))
                relacion.Relacionado = new Asiento
                {
                    IdAsiento = relacion.AsientoRelacionado,
                    Fecha = Convert.ToDateTime(dr["relacionado_fecha"]),
                    Referencia = dr["relacionado_referencia"] != DBNull.Value
                                    ? dr["relacionado_referencia"].ToString() : null,
                    Moneda = dr["relacionado_moneda"]?.ToString() ?? "PEN"
                };

            return relacion;
        }
    }
}