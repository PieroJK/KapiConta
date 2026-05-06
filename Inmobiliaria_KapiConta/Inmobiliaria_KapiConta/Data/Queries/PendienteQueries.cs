namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class PendienteQueries
    {
        public static string ObtenerPendientes = @"
            SELECT 
                sub.asiento_ref, 
                sub.cuenta_cod,
                sub.id_plan_cuenta,
                sub.doc_nro,
                sub.id_tercero,
                sub.ruc_tercero,
                sub.tercero_nom,
                sub.monto_original,
                sub.saldo_actual
            FROM (
                SELECT 
                    FIRST_VALUE(a.referencia) OVER(
                        PARTITION BY ad.serie_comprobante, pc.codigo, t.id_tercero 
                        ORDER BY a.fecha ASC, a.id_asiento ASC
                    ) as asiento_ref,

                    pc.codigo AS cuenta_cod,
                    pc.id_plan_cuenta,
                    ad.serie_comprobante AS doc_nro,
                    t.id_tercero,
                    t.documento AS ruc_tercero,
                    t.razon_social AS tercero_nom,

                    FIRST_VALUE(ad.debe - ad.haber) OVER(
                        PARTITION BY ad.serie_comprobante, pc.codigo, t.id_tercero 
                        ORDER BY a.fecha ASC, a.id_asiento ASC
                    ) as monto_original,

                    SUM(ad.debe - ad.haber) OVER(
                        PARTITION BY ad.serie_comprobante, pc.codigo, t.id_tercero
                    ) AS saldo_actual,

                    ROW_NUMBER() OVER(
                        PARTITION BY ad.serie_comprobante, pc.codigo, t.id_tercero 
                        ORDER BY a.fecha ASC, a.id_asiento ASC
                    ) as rn

                FROM asiento_detalle ad
                INNER JOIN asiento a ON ad.id_asiento = a.id_asiento
                INNER JOIN plan_cuenta pc ON ad.id_plan_cuenta = pc.id_plan_cuenta
                LEFT JOIN tercero t ON ad.id_tercero = t.id_tercero

                WHERE a.id_empresa = @idEmpresa
                  AND pc.analisis = true
                  AND a.estado = true
            ) sub
            WHERE sub.rn = 1 
              AND ABS(sub.saldo_actual) > 0.01
            ORDER BY sub.doc_nro;
        ";
    }
}
