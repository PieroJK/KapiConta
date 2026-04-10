namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class CuentaAutomatizacionDetalleQueries
    {
        // 🔍 LISTAR DETALLE
        public static string Listar = @"
            SELECT 
                cad.id_detalle,
                cad.id_automatizacion, -- 🔥 AQUÍ ESTÁ LA CORRECCIÓN
                cad.id_cuenta_relacionada,
                pc.codigo,
                pc.descripcion,
                cad.tipo_movimiento,
                cad.porcentaje
            FROM cuenta_automatizacion ca
            INNER JOIN cuenta_automatizacion_detalle cad
                ON ca.id_automatizacion = cad.id_automatizacion
            INNER JOIN plan_cuenta pc
                ON pc.id_plan_cuenta = cad.id_cuenta_relacionada
            WHERE ca.id_plan_cuenta = @idPlanCuenta
            ORDER BY cad.id_detalle;
        ";

        // ❌ ELIMINAR DETALLE
        public static string EliminarPorAutomatizacion = @"
            DELETE FROM cuenta_automatizacion_detalle
            WHERE id_automatizacion = @id;
        ";

        // ➕ INSERTAR DETALLE
        public static string Insertar = @"
            INSERT INTO cuenta_automatizacion_detalle
            (id_automatizacion, id_cuenta_relacionada, tipo_movimiento, porcentaje)
            VALUES (@idAuto, @idCuenta, @tipo, @porcentaje);
        ";
    }
}
