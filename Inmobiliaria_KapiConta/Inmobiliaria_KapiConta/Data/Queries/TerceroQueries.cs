namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class TerceroQueries
    {
        public static string Listar = @"
            SELECT 
                t.id_tercero,
                t.documento,
                t.razon_social,
                t.direccion,
                t.estado,
                t.condicion,
                t.id_tercero_tipo_documento,
                td.nombre AS tipo_documento
            FROM tercero t
            INNER JOIN tercero_tipo_documento td 
                ON td.id_tercero_tipo_documento = t.id_tercero_tipo_documento
            WHERE t.estado = true
            ORDER BY t.razon_social;";

        public static string ListarFiltrado = @"
            SELECT 
                t.id_tercero,
                t.documento,
                t.razon_social,
                t.direccion,
                t.estado,
                t.condicion,
                t.id_tercero_tipo_documento,
                td.nombre AS tipo_documento
            FROM tercero t
            INNER JOIN tercero_tipo_documento td 
                ON td.id_tercero_tipo_documento = t.id_tercero_tipo_documento
            WHERE t.estado = true
              AND {0} ILIKE @texto
            ORDER BY t.razon_social;";

        public static string ObtenerPorDocumento = @"
            SELECT *
            FROM tercero
            WHERE documento = @documento
            LIMIT 1;";

        public static string ExisteDocumento = @"
            SELECT COUNT(*) 
            FROM tercero 
            WHERE documento = @documento;";

        public static string Insertar = @"
            INSERT INTO tercero
            (documento, razon_social, direccion, estado, condicion, id_tercero_tipo_documento)
            VALUES
            (@documento, @razon_social, @direccion, @estado, @condicion, @tipo);";

        public static string Actualizar = @"
            UPDATE tercero
            SET razon_social = @razon_social,
                direccion = @direccion,
                estado = @estado,
                condicion = @condicion,
                id_tercero_tipo_documento = @tipo
            WHERE id_tercero = @id;";

        public static string EliminarLogico = @"
            UPDATE tercero
            SET estado = false
            WHERE id_tercero = @id;";

        public static string ObtenerTiposDocumento = @"
            SELECT id_tercero_tipo_documento, cod, nombre
            FROM tercero_tipo_documento
            ORDER BY cod;";
    }
}