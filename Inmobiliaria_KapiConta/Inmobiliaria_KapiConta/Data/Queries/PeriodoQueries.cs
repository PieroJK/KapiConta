namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class PeriodoQueries
    {
        // 🔍 LISTAR POR EMPRESA
        public static string Listar = @"
            SELECT 
                id_periodo,
                anio,
                id_empresa
            FROM periodo
            WHERE id_empresa = @idEmpresa
            ORDER BY anio DESC;
        ";

        // 🔍 OBTENER POR ID
        public static string ObtenerPorId = @"
            SELECT 
                id_periodo,
                anio,
                id_empresa
            FROM periodo
            WHERE id_periodo = @id;
        ";

        // ➕ INSERTAR
        public static string Insertar = @"
            INSERT INTO periodo (anio, id_empresa)
            VALUES (@anio, @idEmpresa)
            RETURNING id_periodo;
        ";

        // ✏️ ACTUALIZAR
        public static string Actualizar = @"
            UPDATE periodo
            SET 
                anio = @anio,
                id_empresa = @idEmpresa
            WHERE id_periodo = @id;
        ";

        public static string ListarConEmpresa = @"
             SELECT 
                 p.id_periodo,
                 p.anio,
                 p.id_empresa,
                 e.nombre AS nombre
             FROM periodo p
             INNER JOIN empresa e ON e.id_empresa = p.id_empresa
             WHERE p.id_empresa = @idEmpresa
             ORDER BY p.anio DESC;
         ";
    }
}