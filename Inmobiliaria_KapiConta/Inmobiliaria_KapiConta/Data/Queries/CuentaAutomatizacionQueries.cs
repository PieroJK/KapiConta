namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class CuentaAutomatizacionQueries
    {
        // 🔍 OBTENER ID AUTOMATIZACIÓN
        public static string ObtenerId = @"
            SELECT id_automatizacion
            FROM cuenta_automatizacion
            WHERE id_plan_cuenta = @idPlanCuenta
            ORDER BY id_automatizacion DESC
            LIMIT 1;
        ";

        // 🔍 OBTENER ESTADO
        public static string ObtenerEstado = @"
            SELECT estado
            FROM cuenta_automatizacion
            WHERE id_plan_cuenta = @idPlanCuenta
            ORDER BY id_automatizacion DESC
            LIMIT 1;
        ";

        // ➕ INSERTAR
        public static string Insertar = @"
            INSERT INTO cuenta_automatizacion (id_plan_cuenta, estado)
            VALUES (@idPlanCuenta, true)
            RETURNING id_automatizacion;
        ";

        // ✏️ ACTIVAR / DESACTIVAR
        public static string ActualizarEstado = @"
            UPDATE cuenta_automatizacion
            SET estado = @estado
            WHERE id_automatizacion = @id;
        ";
    }
}
