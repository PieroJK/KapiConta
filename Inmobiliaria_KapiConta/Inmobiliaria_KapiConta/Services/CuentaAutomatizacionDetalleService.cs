using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class CuentaAutomatizacionDetalleService
    {
        public List<CuentaAutomatizacionDetalle> Listar(int idPlanCuenta)
        {
            var lista = new List<CuentaAutomatizacionDetalle>();

            using var conn = DbConnectionFactory.Create();
            conn.Open();

            using var cmd = new NpgsqlCommand(CuentaAutomatizacionDetalleQueries.Listar, conn);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(CuentaAutomatizacionDetalleMapper.Map(reader));
            }

            return lista;
        }

        public void EliminarPorAutomatizacion(NpgsqlConnection conn, int idAutomatizacion, NpgsqlTransaction tx)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionDetalleQueries.EliminarPorAutomatizacion, conn, tx);
            cmd.Parameters.AddWithValue("@id", idAutomatizacion);
            cmd.ExecuteNonQuery();
        }

        public void Insertar(NpgsqlConnection conn, int idAutomatizacion, CuentaAutomatizacionDetalle item, NpgsqlTransaction tx)
        {
            using var cmd = new NpgsqlCommand(CuentaAutomatizacionDetalleQueries.Insertar, conn, tx);
            cmd.Parameters.AddWithValue("@idAuto", idAutomatizacion);
            cmd.Parameters.AddWithValue("@idCuenta", item.IdCuentaRelacionada);
            cmd.Parameters.AddWithValue("@tipo", item.TipoMovimiento);
            cmd.Parameters.AddWithValue("@porcentaje", item.Porcentaje);

            cmd.ExecuteNonQuery();
        }
    }
}
