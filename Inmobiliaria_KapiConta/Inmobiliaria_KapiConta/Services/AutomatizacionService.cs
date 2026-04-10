using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class AutomatizacionService
    {
        private readonly CuentaAutomatizacionService _autoService = new();
        private readonly CuentaAutomatizacionDetalleService _detalleService = new();

        //VALIDAR
        public void Validar(List<CuentaAutomatizacionDetalle> detalles)
        {
            if (detalles == null || detalles.Count == 0)
                throw new Exception("Agrega al menos una cuenta.");

            decimal debe = detalles
                .Where(x => x.TipoMovimiento == "D")
                .Sum(x => x.Porcentaje);

            decimal haber = detalles
                .Where(x => x.TipoMovimiento == "H")
                .Sum(x => x.Porcentaje);

            if (debe <= 0)
                throw new Exception("Debe existir al menos una línea en Debe.");

            if (haber <= 0)
                throw new Exception("Debe existir al menos una línea en Haber.");

            if (debe != 100m)
                throw new Exception($"Debe = 100. Actual: {debe}");

            if (haber != 100m)
                throw new Exception($"Haber = 100. Actual: {haber}");
        }

        //LISTAR
        public (List<CuentaAutomatizacionDetalle> lista, bool estado) Obtener(int idPlanCuenta)
        {
            using var conn = DbConnectionFactory.Create();
            conn.Open();

            var estado = _autoService.ObtenerEstado(conn, idPlanCuenta);
            var lista = _detalleService.Listar(idPlanCuenta);

            return (lista, estado);
        }

        //CRUD - Guardar/Actualizar/Desactivar
        public void Guardar(int idPlanCuenta, List<CuentaAutomatizacionDetalle> detalles, bool activo)
        {
            using var conn = DbConnectionFactory.Create();
            conn.Open();

            using var tx = conn.BeginTransaction();

            try
            {
                var idAuto = _autoService.ObtenerId(conn, idPlanCuenta, tx);

                // 🔴 DESACTIVAR
                if (!activo)
                {
                    if (idAuto.HasValue)
                        _autoService.ActualizarEstado(conn, idAuto.Value, false, tx);

                    ActualizarFlag(conn, idPlanCuenta, false, tx);

                    tx.Commit();
                    return;
                }

                if (detalles == null || detalles.Count == 0)
                    throw new Exception("No hay detalles.");

                int idFinal = idAuto ?? _autoService.Insertar(conn, idPlanCuenta, tx);

                // 🔹 Activar
                _autoService.ActualizarEstado(conn, idFinal, true, tx);

                // 🔹 Limpiar detalles
                _detalleService.EliminarPorAutomatizacion(conn, idFinal, tx);

                // 🔹 Insertar detalles
                foreach (var item in detalles)
                    _detalleService.Insertar(conn, idFinal, item, tx);

                // 🔹 Flag en plan_cuenta
                ActualizarFlag(conn, idPlanCuenta, true, tx);

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private void ActualizarFlag(NpgsqlConnection conn, int idPlanCuenta, bool estado, NpgsqlTransaction tx)
        {
            using var cmd = new NpgsqlCommand(PlanCuentaQueries.ActualizarFlagAutomatizacion, conn, tx);
            cmd.Parameters.AddWithValue("@id", idPlanCuenta);
            cmd.Parameters.AddWithValue("@estado", estado);

            cmd.ExecuteNonQuery();
        }
    }
}
