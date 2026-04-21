namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class RelacionAsientoQueries
    {
        public static string ListarPorAsiento = @"
            SELECT
                r.id_relacion,
                r.asiento_origen,
                r.asiento_relacionado,
                r.estado,
                ao.fecha        AS origen_fecha,
                ao.referencia   AS origen_referencia,
                ao.moneda       AS origen_moneda,
                ar.fecha        AS relacionado_fecha,
                ar.referencia   AS relacionado_referencia,
                ar.moneda       AS relacionado_moneda
            FROM relacion_asiento r
            INNER JOIN asiento ao ON ao.id_asiento = r.asiento_origen
            INNER JOIN asiento ar ON ar.id_asiento = r.asiento_relacionado
            WHERE r.estado = true
              AND (r.asiento_origen      = @idAsiento
               OR  r.asiento_relacionado = @idAsiento);";

        public static string ObtenerPorId = @"
            SELECT
                r.id_relacion,
                r.asiento_origen,
                r.asiento_relacionado,
                r.estado,
                ao.fecha        AS origen_fecha,
                ao.referencia   AS origen_referencia,
                ao.moneda       AS origen_moneda,
                ar.fecha        AS relacionado_fecha,
                ar.referencia   AS relacionado_referencia,
                ar.moneda       AS relacionado_moneda
            FROM relacion_asiento r
            INNER JOIN asiento ao ON ao.id_asiento = r.asiento_origen
            INNER JOIN asiento ar ON ar.id_asiento = r.asiento_relacionado
            WHERE r.id_relacion = @idRelacion;";

        public static string Insertar = @"
            INSERT INTO relacion_asiento
            (asiento_origen, asiento_relacionado)
            VALUES
            (@asientoOrigen, @asientoRelacionado)
            RETURNING *;";

        public static string EliminarLogico = @"
            UPDATE relacion_asiento
            SET estado = false
            WHERE id_relacion = @idRelacion;";
    }
}