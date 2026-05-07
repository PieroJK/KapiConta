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

        public static string ObtenerReferenciasPorFiltro = @"
    SELECT referencia
    FROM asiento
    WHERE id_empresa = @idEmpresa
      AND id_mes = @idMes
      AND id_sub_diario = @idSubDiario
      AND estado = true;";

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
    a.referencia                                                        AS numero_correlativo,
    a.fecha                                                             AS fecha_emision,
    a.fecha_ven,
    MAX(tf.cod)                                                         AS tipo_doc,
    MAX(ad.serie_comprobante)                                           AS serie_numero,
    MAX(ttd.cod)                                                        AS tipo_doc_cliente,
    MAX(t.documento)                                                    AS nro_documento,
    MAX(t.razon_social)                                                 AS razon_social,

    -- ✅ COLUMNAS POR OPERACIÓN (pivoteo manual)
    SUM(CASE WHEN toa.codigo = 'GRA1' AND pc.codigo NOT LIKE '40%' 
             THEN ad.debe ELSE 0 END)                                   AS gra1,

    SUM(
    CASE 
        WHEN toa.codigo = 'IGV1'
         AND pc.codigo LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS igv1,

   SUM(
    CASE 
        WHEN toa.codigo = 'GRA2'
         AND pc.codigo NOT LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS gra2,

SUM(
    CASE 
        WHEN toa.codigo = 'IGV2'
         AND pc.codigo LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS igv2,

SUM(
    CASE 
        WHEN toa.codigo = 'GRA3'
         AND pc.codigo NOT LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS gra3,

SUM(
    CASE 
        WHEN toa.codigo = 'IGV3'
         AND pc.codigo LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS igv3,

SUM(
    CASE 
        WHEN toa.codigo = 'GRA4'
         AND pc.codigo NOT LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS gra4,

SUM(
    CASE 
        WHEN toa.codigo = 'IGV4'
         AND pc.codigo LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS igv4,

SUM(
    CASE 
        WHEN toa.codigo = 'GRA5'
         AND pc.codigo NOT LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS gra5,

SUM(
    CASE 
        WHEN toa.codigo = 'IGV5'
         AND pc.codigo LIKE '40%' 
        THEN 
            CASE
                WHEN a.id_sub_diario = 1
                    THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END
) AS igv5,

    SUM(CASE 
        WHEN toa.codigo = 'EXO'
         AND pc.codigo NOT LIKE '40%'
        THEN 
            CASE 
                WHEN a.id_sub_diario = 1 THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END) AS exonerada,

    SUM(CASE 
        WHEN toa.codigo = 'INA'
         AND pc.codigo NOT LIKE '40%'
        THEN 
            CASE 
                WHEN a.id_sub_diario = 1 THEN ad.haber
                ELSE ad.debe
            END
        ELSE 0
    END) AS inafecta,

    SUM(CASE WHEN pc.codigo LIKE '40%' 
             THEN ad.debe ELSE 0 END)                                   AS igv_total,

    SUM(ad.debe)                                                        AS importe_total,

    MAX(CASE WHEN ad.serie_comprobante IS NOT NULL 
             THEN ad.glosa ELSE NULL END)                               AS glosa,

    a.moneda,
    CASE WHEN a.moneda = 'PEN' THEN 1 ELSE MAX(tc.venta) END           AS tipo_cambio,
    MAX(u.usuario)                                                      AS usuario,
    a.fecha_modificacion,
    MAX(a_rel.referencia)                                               AS comprobante_modificado

FROM asiento_detalle ad
INNER JOIN asiento               a    ON a.id_asiento            = ad.id_asiento
INNER JOIN plan_cuenta           pc   ON pc.id_plan_cuenta       = ad.id_plan_cuenta
LEFT  JOIN tipo_operacion_asiento toa  ON toa.id_tipo_operacion  = ad.id_tipo_operacion
LEFT  JOIN tipo_facturacion      tf   ON tf.id_tipo_facturacion  = ad.id_tipo_facturacion
LEFT  JOIN tercero               t    ON t.id_tercero            = ad.id_tercero
LEFT  JOIN tercero_tipo_documento ttd  ON ttd.id_tercero_tipo_documento = t.id_tercero_tipo_documento
LEFT  JOIN relacion_asiento      ar   ON ar.id_relacion          = ad.id_relacion
LEFT  JOIN asiento               a_rel ON a_rel.id_asiento       = ar.asiento_relacionado
LEFT  JOIN tipo_cambio           tc   ON tc.id_tipo_cambio       = a.id_tipo_cambio
LEFT  JOIN usuario               u    ON u.id_usuario            = a.id_usuario

WHERE a.id_empresa    = @idEmpresa
  AND a.estado        = true
  AND a.id_mes        = @idMes
  AND a.id_sub_diario = @idSubDiario

GROUP BY
    a.id_asiento,
    a.referencia,
    a.fecha,
    a.fecha_ven,
    a.moneda,
    a.fecha_modificacion

ORDER BY
    CAST(SUBSTRING(a.referencia FROM 3) AS BIGINT);";

        public static string ObtenerIdPorReferencia = @"
    SELECT id_asiento
    FROM asiento
    WHERE referencia = @referencia
      AND estado = true
    ORDER BY id_asiento DESC
    LIMIT 1;";

        public static string InsertarRelacion = @"
    INSERT INTO relacion_asiento
    (
        asiento_origen,
        asiento_relacionado,
        estado
    )
    VALUES
    (
        @asiento_origen,
        @asiento_relacionado,
        true
    )
    RETURNING id_relacion;";
    }
}
