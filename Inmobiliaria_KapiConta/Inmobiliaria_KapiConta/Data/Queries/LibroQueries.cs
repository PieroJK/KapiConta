namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class LibroQueries
    {
        public static string Listar = @"
            SELECT id_libro, cod, nombre
            FROM libro
            ORDER BY cod;";

        public static string ObtenerPorId = @"
            SELECT id_libro, cod, nombre
            FROM libro
            WHERE id_libro = @idLibro;";
    }
}
