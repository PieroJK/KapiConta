namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class EmpresaQueries
    {
        // LISTAR EMPRESAS ACTIVAS
        public static string Listar = @"
            SELECT 
                id_empresa,
                ruc,
                nombre,
                direccion,
                estado
            FROM empresa
            WHERE estado = true
            ORDER BY nombre;
        ";

        // OBTENER POR ID
        public static string ObtenerPorId = @"
            SELECT 
                id_empresa,
                ruc,
                nombre,
                direccion,
                estado
            FROM empresa
            WHERE id_empresa = @id;
        ";

        // ➕ INSERTAR
        public static string Insertar = @"
            INSERT INTO empresa (ruc, nombre, direccion, estado)
            VALUES (@ruc, @nombre, @direccion, @estado)
            RETURNING id_empresa;
        ";

        // ✏️ ACTUALIZAR
        public static string Actualizar = @"
            UPDATE empresa
            SET 
                ruc = @ruc,
                nombre = @nombre,
                direccion = @direccion,
                estado = @estado
            WHERE id_empresa = @id;
        ";

        // ❌ ELIMINAR LÓGICO
        public static string Eliminar = @"
            UPDATE empresa
            SET estado = false
            WHERE id_empresa = @id;
        ";
    }
}