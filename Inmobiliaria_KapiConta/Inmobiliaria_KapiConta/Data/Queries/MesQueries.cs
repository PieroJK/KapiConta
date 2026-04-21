namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class MesQueries
    {
        public static string Listar = @"
            SELECT id_mes, mes, nombre
            FROM mes
            ORDER BY mes;";

        public static string ObtenerPorId = @"
            SELECT id_mes, mes, nombre
            FROM mes
            WHERE id_mes = @idMes;";
    }
}