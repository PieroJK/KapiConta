using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class CuentaAutomatizacionMapper
    {
        public static CuentaAutomatizacion Map(NpgsqlDataReader reader)
        {
            return new CuentaAutomatizacion
            {
                IdAutomatizacion = (int)reader["id_automatizacion"],
                IdPlanCuenta = (int)reader["id_plan_cuenta"],
                Estado = (bool)reader["estado"]
            };
        }
    }
}
