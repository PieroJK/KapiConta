namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class BalanceQueries
    {
        public static string Listar = @"
            SELECT id_balance, nombre
            FROM balance
            ORDER BY id_balance;
        ";
    }
}