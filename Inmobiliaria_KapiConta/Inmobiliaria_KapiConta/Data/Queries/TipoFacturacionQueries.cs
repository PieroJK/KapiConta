namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class TipoFacturacionQueries
    {
        public static string Listar = @"
            SELECT id_tipo_facturacion, cod, nombre, estado
            FROM tipo_facturacion
            WHERE estado = true
            ORDER BY cod;";

        public static string ObtenerPorId = @"
            SELECT id_tipo_facturacion, cod, nombre, estado
            FROM tipo_facturacion
            WHERE id_tipo_facturacion = @idTipoFacturacion;";
    }
}
