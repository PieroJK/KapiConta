namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class PlanCuentaQueries
    {
        // 🔍 VALIDAR EXISTENCIA DE CÓDIGO
        public static string ExisteCodigo = @"
            SELECT COUNT(*)
            FROM plan_cuenta
            WHERE id_empresa = @idEmpresa
              AND codigo = @codigo
              AND (@idExcluir IS NULL OR id_plan_cuenta <> @idExcluir);
        ";

        // 🔍 LISTAR CUENTAS
        public static string Listar = @"
            SELECT 
                id_plan_cuenta,
                id_empresa,
                id_plan_cuenta_base,
                codigo,
                descripcion,
                nivel,
                codigo_padre,
                id_elemento,
                id_balance,
                analisis,
                es_base,
                tiene_automatizacion,
                estado
            FROM plan_cuenta
            WHERE id_empresa = @idEmpresa
              AND estado = true
            ORDER BY codigo;
        ";

        // 🔍 CUENTAS PADRE (para combos)
        public static string ListarPadres = @"
            SELECT codigo, descripcion
            FROM plan_cuenta
            WHERE id_empresa = @idEmpresa
              AND estado = true
            ORDER BY codigo;
        ";

        // ➕ INSERTAR
        public static string Insertar = @"
            INSERT INTO plan_cuenta
            (id_empresa, id_plan_cuenta_base, codigo, descripcion, nivel,
             codigo_padre, id_elemento, id_balance, analisis,
             es_base, tiene_automatizacion, estado)
            VALUES
            (@idEmpresa, @idPlanCuentaBase, @codigo, @descripcion, @nivel,
             @codigoPadre, @idElemento, @idBalance, @analisis,
             false, false, true);
        ";

        // ✏️ ACTUALIZAR
        public static string Actualizar = @"
            UPDATE plan_cuenta
            SET codigo = @codigo,
                descripcion = @descripcion,
                nivel = @nivel,
                codigo_padre = @codigoPadre,
                id_elemento = @idElemento,
                id_balance = @idBalance,
                analisis = @analisis
            WHERE id_plan_cuenta = @idPlanCuenta
              AND id_empresa = @idEmpresa;
        ";

        // ❌ ELIMINAR LÓGICO
        public static string Eliminar = @"
            UPDATE plan_cuenta
            SET estado = false
            WHERE id_plan_cuenta = @idPlanCuenta
              AND id_empresa = @idEmpresa;
        ";

        // 🔄 ACTUALIZAR FLAG AUTOMATIZACIÓN
        public static string ActualizarFlagAutomatizacion = @"
            UPDATE plan_cuenta
            SET tiene_automatizacion = @estado
            WHERE id_plan_cuenta = @id;
        ";
    }
}