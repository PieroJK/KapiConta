using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class PlanCuentaMapper
    {
        public static PlanCuenta Map(NpgsqlDataReader reader)
        {
            var plan = new PlanCuenta
            {
                IdPlanCuenta = (int)reader["id_plan_cuenta"],
                IdEmpresa = (int)reader["id_empresa"],
                IdPlanCuentaBase = reader["id_plan_cuenta_base"] != DBNull.Value
                                        ? (int)reader["id_plan_cuenta_base"]
                                        : null,
                Codigo = reader["codigo"]?.ToString() ?? string.Empty,
                Descripcion = reader["descripcion"]?.ToString() ?? string.Empty,
                Nivel = Convert.ToInt32(reader["nivel"]),
                CodigoPadre = reader["codigo_padre"]?.ToString(),
                IdElemento = (int)reader["id_elemento"],
                IdBalance = reader["id_balance"] != DBNull.Value
                                ? (int)reader["id_balance"]
                                : null,
                Analisis = (bool)reader["analisis"],
                EsBase = (bool)reader["es_base"],
                TieneAutomatizacion = (bool)reader["tiene_automatizacion"],
                Estado = (bool)reader["estado"]
            };

            // 🔥 Empresa (si haces JOIN)
            if (reader.HasColumn("empresa_nombre"))
            {
                plan.Empresa = new Empresa
                {
                    IdEmpresa = plan.IdEmpresa,
                    Nombre = reader["empresa_nombre"]?.ToString() ?? string.Empty
                };
            }

            // 🔥 Elemento
            if (reader.HasColumn("elemento_nombre"))
            {
                plan.Elemento = new Elemento
                {
                    IdElemento = plan.IdElemento,
                    Nombre = reader["elemento_nombre"]?.ToString() ?? string.Empty
                };
            }

            // 🔥 Balance
            if (reader.HasColumn("balance_nombre") && plan.IdBalance != null)
            {
                plan.Balance = new Balance
                {
                    IdBalance = plan.IdBalance.Value,
                    Nombre = reader["balance_nombre"]?.ToString() ?? string.Empty
                };
            }

            // 🔥 Plan base
            if (reader.HasColumn("base_codigo") && plan.IdPlanCuentaBase != null)
            {
                plan.PlanCuentaBase = new PlanCuentaBase
                {
                    IdPlanCuentaBase = plan.IdPlanCuentaBase.Value,
                    Codigo = reader["base_codigo"]?.ToString() ?? string.Empty,
                    Descripcion = reader["base_descripcion"]?.ToString() ?? string.Empty
                };
            }



            return plan;
        }
    }
}