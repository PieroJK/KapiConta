namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class AsientoDetalleQueries
    {
        public static string ListarPorAsiento = @"
            SELECT
                ad.id_asiento_detalle,
                ad.id_asiento,
                ad.id_plan_cuenta,
                ad.moneda,
                ad.debe,
                ad.haber,
                ad.id_tipo_facturacion,
                ad.serie_comprobante,
                ad.id_tercero,
                ad.glosa,
                ad.id_relacion,
                ad.id_tipo_operacion,
                ad.id_costo,
                pc.codigo           AS plan_cuenta_codigo,
                pc.descripcion      AS plan_cuenta_descripcion,
                tf.nombre           AS tipo_facturacion_nombre,
                tf.cod              AS tipo_facturacion_cod,
                t.razon_social      AS tercero_razon_social,
                t.documento         AS tercero_documento,
                top.nombre          AS tipo_operacion_nombre,
                top.codigo          AS tipo_operacion_codigo,
                c.codigo            AS costo_codigo,
                c.descripcion       AS costo_descripcion
            FROM asiento_detalle ad
            INNER JOIN plan_cuenta          pc  ON pc.id_plan_cuenta       = ad.id_plan_cuenta
            LEFT  JOIN tipo_facturacion     tf  ON tf.id_tipo_facturacion  = ad.id_tipo_facturacion
            LEFT  JOIN tercero              t   ON t.id_tercero            = ad.id_tercero
            LEFT  JOIN tipo_operacion_asiento top ON top.id_tipo_operacion = ad.id_tipo_operacion
            LEFT  JOIN costo                c   ON c.id_costo             = ad.id_costo
            WHERE ad.id_asiento = @idAsiento
            ORDER BY ad.id_asiento_detalle;";

        public static string ObtenerPorId = @"
            SELECT
                ad.id_asiento_detalle,
                ad.id_asiento,
                ad.id_plan_cuenta,
                ad.moneda,
                ad.debe,
                ad.haber,
                ad.id_tipo_facturacion,
                ad.serie_comprobante,
                ad.id_tercero,
                ad.glosa,
                ad.id_relacion,
                ad.id_tipo_operacion,
                ad.id_costo,
                pc.codigo           AS plan_cuenta_codigo,
                pc.descripcion      AS plan_cuenta_descripcion,
                tf.nombre           AS tipo_facturacion_nombre,
                tf.cod              AS tipo_facturacion_cod,
                t.razon_social      AS tercero_razon_social,
                t.documento         AS tercero_documento,
                top.nombre          AS tipo_operacion_nombre,
                top.codigo          AS tipo_operacion_codigo,
                c.codigo            AS costo_codigo,
                c.descripcion       AS costo_descripcion
            FROM asiento_detalle ad
            INNER JOIN plan_cuenta          pc  ON pc.id_plan_cuenta       = ad.id_plan_cuenta
            LEFT  JOIN tipo_facturacion     tf  ON tf.id_tipo_facturacion  = ad.id_tipo_facturacion
            LEFT  JOIN tercero              t   ON t.id_tercero            = ad.id_tercero
            LEFT  JOIN tipo_operacion_asiento top ON top.id_tipo_operacion = ad.id_tipo_operacion
            LEFT  JOIN costo                c   ON c.id_costo             = ad.id_costo
            WHERE ad.id_asiento_detalle = @idAsientoDetalle;";

        public static string Insertar = @"
            INSERT INTO asiento_detalle
            (id_asiento, id_plan_cuenta, moneda, debe, haber,
             id_tipo_facturacion, serie_comprobante, id_tercero,
             glosa, id_relacion, id_tipo_operacion, id_costo)
            VALUES
            (@idAsiento, @idPlanCuenta, @moneda, @debe, @haber,
             @idTipoFacturacion, @serieComprobante, @idTercero,
             @glosa, @idRelacion, @idTipoOperacion, @idCosto)
            RETURNING *;";

        public static string Actualizar = @"
            UPDATE asiento_detalle
            SET id_plan_cuenta      = @idPlanCuenta,
                moneda              = @moneda,
                debe                = @debe,
                haber               = @haber,
                id_tipo_facturacion = @idTipoFacturacion,
                serie_comprobante   = @serieComprobante,
                id_tercero          = @idTercero,
                glosa               = @glosa,
                id_relacion         = @idRelacion,
                id_tipo_operacion   = @idTipoOperacion,
                id_costo            = @idCosto
            WHERE id_asiento_detalle = @idAsientoDetalle;";

        public static string Eliminar = @"
            DELETE FROM asiento_detalle
            WHERE id_asiento_detalle = @idAsientoDetalle;";

        public static string EliminarPorAsiento = @"
            DELETE FROM asiento_detalle
            WHERE id_asiento = @idAsiento;";
    }
}
