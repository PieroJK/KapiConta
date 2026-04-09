using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class PlanCuentaBaseMapper
    {
        public static PlanCuentaBase Map(NpgsqlDataReader reader)
        {
            var plan = new PlanCuentaBase
            {
                IdPlanCuentaBase = (int)reader["id_plan_cuenta_base"],
                Codigo = reader["codigo"]?.ToString() ?? string.Empty,
                Descripcion = reader["descripcion"]?.ToString() ?? string.Empty,
                Nivel = reader["nivel"]?.ToString() ?? string.Empty,
                CodigoPadre = reader["codigo_padre"]?.ToString(),
                IdElemento = (int)reader["id_elemento"],
                IdBalance = reader["id_balance"] != DBNull.Value
                                ? (int)reader["id_balance"]
                                : null,
                Analisis = (bool)reader["analisis"],
                Estado = (bool)reader["estado"]
            };

            // 🔥 Elemento (si viene en JOIN)
            if (reader.HasColumn("elemento_nombre"))
            {
                plan.Elemento = new Elemento
                {
                    IdElemento = plan.IdElemento,
                    Nombre = reader["elemento_nombre"]?.ToString() ?? string.Empty
                };
            }

            // 🔥 Balance (si viene en JOIN)
            if (reader.HasColumn("balance_nombre") && plan.IdBalance != null)
            {
                plan.Balance = new Balance
                {
                    IdBalance = plan.IdBalance.Value,
                    Nombre = reader["balance_nombre"]?.ToString() ?? string.Empty
                };
            }

            return plan;
        }
    }
}
