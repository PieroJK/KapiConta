namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class SubDiarioQueries
    {
        public static string Listar = @"
            SELECT id_sub_diario, diario, nombre
            FROM sub_diario
            ORDER BY diario;";

        public static string ObtenerPorId = @"
            SELECT id_sub_diario, diario, nombre
            FROM sub_diario
            WHERE id_sub_diario = @idSubDiario;";
    }
}