using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class CuentaAutomatizacionService
    {
        public int? ObtenerId(NpgsqlConnection conn, int idPlanCuenta, NpgsqlTransaction tx = null)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionQueries.ObtenerId, conn, tx);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

            var result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value
                ? (int?)Convert.ToInt32(result)
                : null;
        }

        public bool ObtenerEstado(NpgsqlConnection conn, int idPlanCuenta)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionQueries.ObtenerEstado, conn);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

            var result = cmd.ExecuteScalar();

            return result != null && result != DBNull.Value && (bool)result;
        }

        public int Insertar(NpgsqlConnection conn, int idPlanCuenta, NpgsqlTransaction tx)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionQueries.Insertar, conn, tx);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void ActualizarEstado(NpgsqlConnection conn, int id, bool estado, NpgsqlTransaction tx)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionQueries.ActualizarEstado, conn, tx);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@estado", estado);

            cmd.ExecuteNonQuery();
        }
    }
}
