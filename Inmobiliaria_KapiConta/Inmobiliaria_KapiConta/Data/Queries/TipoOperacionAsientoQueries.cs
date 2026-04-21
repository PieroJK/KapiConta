namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class TipoOperacionAsientoQueries
    {
        public static string Listar = @"
            SELECT id_tipo_operacion, codigo, nombre, estado
            FROM tipo_operacion_asiento
            WHERE estado = true
            ORDER BY codigo;";

        public static string ObtenerPorId = @"
            SELECT id_tipo_operacion, codigo, nombre, estado
            FROM tipo_operacion_asiento
            WHERE id_tipo_operacion = @idTipoOperacion;";
    }
}
