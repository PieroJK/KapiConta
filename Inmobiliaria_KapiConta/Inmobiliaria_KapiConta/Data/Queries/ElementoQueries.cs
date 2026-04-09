namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class ElementoQueries
    {
        public static string Listar = @"
            SELECT id_elemento, nombre
            FROM elemento
            ORDER BY id_elemento;
        ";
    }
}
