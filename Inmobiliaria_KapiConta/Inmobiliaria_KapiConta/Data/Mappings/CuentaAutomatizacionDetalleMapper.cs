using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class CuentaAutomatizacionDetalleMapper
    {
        public static CuentaAutomatizacionDetalle Map(NpgsqlDataReader reader)
        {
            var detalle = new CuentaAutomatizacionDetalle
            {
                IdDetalle = (int)reader["id_detalle"],
                IdAutomatizacion = (int)reader["id_automatizacion"],
                IdCuentaRelacionada = (int)reader["id_cuenta_relacionada"],
                TipoMovimiento = reader["tipo_movimiento"]?.ToString() ?? string.Empty,
                Porcentaje = (decimal)reader["porcentaje"]
            };

            // 🔥 Si haces JOIN con plan_cuenta
            if (reader.HasColumn("codigo"))
            {
                detalle.CuentaRelacionada = new PlanCuenta
                {
                    IdPlanCuenta = detalle.IdCuentaRelacionada,
                    Codigo = reader["codigo"]?.ToString() ?? string.Empty,
                    Descripcion = reader["descripcion"]?.ToString() ?? string.Empty
                };
            }

            return detalle;
        }
    }
}
