namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class CostoQueries
    {
        public static string ListarPorEmpresa = @"
            SELECT
                c.id_costo,
                c.id_empresa,
                c.codigo,
                c.descripcion,
                c.estado,
                e.nombre AS empresa_nombre
            FROM costo c
            INNER JOIN empresa e ON e.id_empresa = c.id_empresa
            WHERE c.estado     = true
              AND c.id_empresa = @idEmpresa
            ORDER BY c.codigo;";

        public static string ObtenerPorId = @"
            SELECT
                c.id_costo,
                c.id_empresa,
                c.codigo,
                c.descripcion,
                c.estado,
                e.nombre AS empresa_nombre
            FROM costo c
            INNER JOIN empresa e ON e.id_empresa = c.id_empresa
            WHERE c.id_costo = @idCosto;";

        public static string ExisteCodigo = @"
            SELECT COUNT(*)
            FROM costo
            WHERE id_empresa = @idEmpresa
              AND codigo     = @codigo;";

        public static string ExisteCodigoExcluyendo = @"
            SELECT COUNT(*)
            FROM costo
            WHERE id_empresa = @idEmpresa
              AND codigo     = @codigo
              AND id_costo  <> @idExcluir;";

        public static string Insertar = @"
            INSERT INTO costo
            (id_empresa, codigo, descripcion)
            VALUES
            (@idEmpresa, @codigo, @descripcion)
            RETURNING *;";

        public static string Actualizar = @"
            UPDATE costo
            SET codigo      = @codigo,
                descripcion = @descripcion
            WHERE id_costo   = @idCosto
              AND id_empresa = @idEmpresa;";

        public static string EliminarLogico = @"
            UPDATE costo
            SET estado = false
            WHERE id_costo   = @idCosto
              AND id_empresa = @idEmpresa;";
    }
}
