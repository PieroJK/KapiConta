namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class AsientoQueries
    {
        public static string Listar = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.estado = true
            ORDER BY a.fecha DESC;";

        public static string ListarPorEmpresaYPeriodo = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.estado      = true
              AND a.id_empresa  = @idEmpresa
              AND a.id_periodo  = @idPeriodo
            ORDER BY a.fecha DESC;";

        public static string ObtenerPorId = @"
            SELECT
                a.id_asiento,
                a.id_empresa,
                a.id_mes,
                a.id_sub_diario,
                a.referencia,
                a.id_libro,
                a.fecha,
                a.moneda,
                a.id_tipo_cambio,
                a.fecha_ven,
                a.id_usuario,
                a.fecha_modificacion,
                a.estado,
                a.id_periodo,
                e.nombre        AS empresa_nombre,
                m.nombre        AS mes_nombre,
                sd.nombre       AS sub_diario_nombre,
                l.nombre        AS libro_nombre,
                tc.compra       AS tipo_cambio_compra,
                tc.venta        AS tipo_cambio_venta,
                tc.fecha        AS tipo_cambio_fecha,
                u.username      AS usuario_nombre,
                p.anio          AS periodo_anio
            FROM asiento a
            INNER JOIN empresa     e  ON e.id_empresa       = a.id_empresa
            INNER JOIN mes         m  ON m.id_mes           = a.id_mes
            INNER JOIN sub_diario  sd ON sd.id_sub_diario   = a.id_sub_diario
            INNER JOIN libro       l  ON l.id_libro         = a.id_libro
            INNER JOIN periodo     p  ON p.id_periodo       = a.id_periodo
            LEFT  JOIN tipo_cambio tc ON tc.id_tipo_cambio  = a.id_tipo_cambio
            LEFT  JOIN usuario     u  ON u.id_usuario       = a.id_usuario
            WHERE a.id_asiento = @idAsiento;";

        public static string Insertar = @"
            INSERT INTO asiento
            (id_empresa, id_mes, id_sub_diario, referencia, id_libro,
             fecha, moneda, id_tipo_cambio, fecha_ven, id_usuario, id_periodo)
            VALUES
            (@idEmpresa, @idMes, @idSubDiario, @referencia, @idLibro,
             @fecha, @moneda, @idTipoCambio, @fechaVen, @idUsuario, @idPeriodo)
            RETURNING *;";

        public static string Actualizar = @"
            UPDATE asiento
            SET id_mes          = @idMes,
                id_sub_diario   = @idSubDiario,
                referencia      = @referencia,
                id_libro        = @idLibro,
                fecha           = @fecha,
                moneda          = @moneda,
                id_tipo_cambio  = @idTipoCambio,
                fecha_ven       = @fechaVen,
                fecha_modificacion = current_timestamp
            WHERE id_asiento  = @idAsiento
              AND id_empresa  = @idEmpresa;";

        public static string EliminarLogico = @"
            UPDATE asiento
            SET estado = false
            WHERE id_asiento = @idAsiento
              AND id_empresa = @idEmpresa;";

        public static string ListarAsientos = @"
            SELECT 
    a.referencia AS numero_correlativo,
    a.fecha AS fecha_emision,
    a.fecha_ven,
    tf.cod AS tipo_doc,
    ad.serie_comprobante AS serie_numero,
    ttd.cod AS tipo_doc_cliente,
    t.documento AS nro_documento,
    t.razon_social,
-- Agrega esta línea al SELECT después de STRING_AGG:
SUM(CASE WHEN pc.codigo NOT LIKE '40%' AND ad.debe > 0 THEN ad.debe ELSE 0 END) AS debe,
    SUM(CASE WHEN pc.codigo NOT LIKE '40%' THEN ad.debe ELSE 0 END) AS base_total,
    CASE WHEN MAX(toa.codigo) = 'GRA1' THEN SUM(CASE WHEN pc.codigo NOT LIKE '40%' THEN ad.debe ELSE 0 END) ELSE 0 END AS base_imponible,
    CASE WHEN MAX(toa.codigo) = 'EXO'  THEN SUM(CASE WHEN pc.codigo NOT LIKE '40%' THEN ad.debe ELSE 0 END) ELSE 0 END AS exonerada,
    CASE WHEN MAX(toa.codigo) = 'INA'  THEN SUM(CASE WHEN pc.codigo NOT LIKE '40%' THEN ad.debe ELSE 0 END) ELSE 0 END AS inafecta,
    SUM(CASE WHEN toa.codigo = 'VIM'   THEN ad.debe ELSE 0 END) AS valor_imponible,
    SUM(CASE WHEN pc.codigo LIKE '40%' THEN ad.debe ELSE 0 END) AS igv,
    SUM(CASE WHEN pc.codigo LIKE '403%' THEN ad.debe ELSE 0 END) AS isc,
    SUM(ad.debe) AS importe_total,
    ad.glosa AS glosa,
    STRING_AGG(DISTINCT toa.codigo, ', ') AS operacion,  -- ✅ lista de operaciones
    a.moneda,
    CASE WHEN a.moneda = 'PEN' THEN 1 ELSE tc.venta END AS tipo_cambio,
    u.usuario,
    a.fecha_modificacion,
    a_rel.referencia AS comprobante_modificado
FROM asiento_detalle ad
INNER JOIN asiento a ON ad.id_asiento = a.id_asiento
INNER JOIN plan_cuenta pc ON ad.id_plan_cuenta = pc.id_plan_cuenta
INNER JOIN sub_diario sd ON a.id_sub_diario = sd.id_sub_diario
LEFT JOIN tipo_operacion_asiento toa ON ad.id_tipo_operacion = toa.id_tipo_operacion
LEFT JOIN tipo_facturacion tf ON ad.id_tipo_facturacion = tf.id_tipo_facturacion
LEFT JOIN tercero t ON ad.id_tercero = t.id_tercero
LEFT JOIN tercero_tipo_documento ttd ON t.id_tercero_tipo_documento = ttd.id_tercero_tipo_documento
LEFT JOIN relacion_asiento ar ON ad.id_relacion = ar.id_relacion
LEFT JOIN asiento a_rel ON ar.asiento_relacionado = a_rel.id_asiento
LEFT JOIN tipo_cambio tc ON a.id_tipo_cambio = tc.id_tipo_cambio
LEFT JOIN usuario u ON a.id_usuario = u.id_usuario
WHERE a.id_empresa = @idEmpresa 
  AND a.estado = true
  AND a.id_mes = @idMes
  AND sd.diario = @subDiario
GROUP BY 
    a.referencia, a.fecha, a.fecha_ven, tf.cod, ad.serie_comprobante,
    ttd.cod, t.documento, t.razon_social, ad.glosa,
    a.moneda, tc.venta, u.usuario, a.fecha_modificacion, a_rel.referencia
ORDER BY 
    CAST(SUBSTRING(a.referencia FROM 2) AS BIGINT), a.fecha;";
    }
}
