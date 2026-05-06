using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Inmobiliaria_KapiConta.Services
{
   public class AsientoService
    {
        public int GuardarAsiento(Asiento asiento, List<AsientoDetalle> detalles)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var tx = cn.BeginTransaction();
            MessageBox.Show("SERVICE: Guardando asiento");
            try
            {
                // 1. Insertar cabecera
                int idAsiento = InsertarCabecera(cn, tx, asiento);

                // 2. Preparar relaciones (nota crédito/débito)
                var relacionesPorReferencia = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                var grupos = detalles
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x.SerieComprobante) && // 🔁 aquí cambias referencia
                        (x.TipoFacturacion?.Cod == "07" || x.TipoFacturacion?.Cod == "08"))
                    .GroupBy(x => x.SerieComprobante!.Trim());

                foreach (var grupo in grupos)
                {
                    string referencia = grupo.Key;

                    int idAsientoRelacionado = ObtenerIdAsientoPorReferencia(
                        cn, tx, referencia);

                    int idRelacion = InsertarRelacionAsiento(
                        cn, tx, idAsiento, idAsientoRelacionado);

                    relacionesPorReferencia[referencia] = idRelacion;
                }

                // 3. Insertar detalles
                foreach (var item in detalles)
                {
                    int? idRelacion = null;

                    if (!string.IsNullOrWhiteSpace(item.SerieComprobante) &&
                        relacionesPorReferencia.TryGetValue(item.SerieComprobante.Trim(), out int rel))
                    {
                        idRelacion = rel;
                    }

                    InsertarDetalle(cn, tx, idAsiento, item, idRelacion);
                }

                tx.Commit();
                return idAsiento;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public DataTable ObtenerAsientos(int idEmpresa, int idMes, int idSubDiario)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(AsientoQueries.ListarAsientos, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@idSubDiario", idSubDiario);

            using var reader = cmd.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public string ObtenerSiguienteReferencia(
            int idEmpresa,
            int idMes,
            int idSubDiario,
            string codigoSubDiario,
            string codigoMes)
        {
            var correlativos = new List<int>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(
                AsientoQueries.ObtenerReferenciasPorFiltro, cn);

            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@idSubDiario", idSubDiario);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                string referencia = dr["referencia"]?.ToString() ?? "";

                // Formato esperado: XXY000001
                if (referencia.Length >= 9)
                {
                    string numeroTexto = referencia.Substring(3);

                    if (int.TryParse(numeroTexto, out int numero))
                        correlativos.Add(numero);
                }
            }

            int siguiente = ObtenerMenorLibre(correlativos);

            return $"{codigoSubDiario}{codigoMes}{siguiente:000000}";
        }

        // =========================
        // LÓGICA INTERNA
        // =========================

        private int ObtenerMenorLibre(List<int> lista)
        {
            if (!lista.Any()) return 1;

            lista.Sort();

            int esperado = 1;

            foreach (var num in lista)
            {
                if (num != esperado)
                    return esperado;

                esperado++;
            }

            return esperado;
        }

        private int InsertarCabecera(
    NpgsqlConnection cn,
    NpgsqlTransaction tx,
    Asiento asiento)
        {
            using var cmd = new NpgsqlCommand(AsientoQueries.Insertar, cn, tx);

            cmd.Parameters.AddWithValue("@idEmpresa", asiento.IdEmpresa);
            cmd.Parameters.AddWithValue("@idMes", asiento.IdMes);
            cmd.Parameters.AddWithValue("@idSubDiario", asiento.IdSubDiario);
            cmd.Parameters.AddWithValue("@referencia", (object?)asiento.Referencia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idLibro", asiento.IdLibro);
            cmd.Parameters.AddWithValue("@fecha", asiento.Fecha.Date);
            cmd.Parameters.AddWithValue("@moneda", asiento.Moneda);
            cmd.Parameters.AddWithValue("@idPeriodo", asiento.IdPeriodo);

            cmd.Parameters.AddWithValue("@idTipoCambio",
                (object?)asiento.IdTipoCambio ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@fechaVen",
                (object?)asiento.FechaVen ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@idUsuario",
                (object?)asiento.IdUsuario ?? DBNull.Value);

            int idAsiento;

            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    throw new Exception("No se pudo insertar el asiento.");

                var nuevo = Inmobiliaria_KapiConta.Data.Mappings.AsientoMapper.Map(reader);
                idAsiento = nuevo.IdAsiento;
            } // 👈 AQUÍ se cierra el reader

            return idAsiento;
        }

        private int ObtenerIdAsientoPorReferencia(
    NpgsqlConnection cn,
    NpgsqlTransaction tx,
    string referencia)
        {
            using var cmd = new NpgsqlCommand(
                AsientoQueries.ObtenerIdPorReferencia, cn, tx);

            cmd.Parameters.AddWithValue("@referencia", referencia);

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                throw new Exception($"No se encontró el asiento de referencia: {referencia}");

            return Convert.ToInt32(result);
        }

        private int InsertarRelacionAsiento(
    NpgsqlConnection cn,
    NpgsqlTransaction tx,
    int asientoOrigen,
    int asientoRelacionado)
        {
            using var cmd = new NpgsqlCommand(
                AsientoQueries.InsertarRelacion, cn, tx);

            cmd.Parameters.AddWithValue("@asiento_origen", asientoOrigen);
            cmd.Parameters.AddWithValue("@asiento_relacionado", asientoRelacionado);

            var result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                throw new Exception("No se pudo insertar la relación de asiento.");

            return Convert.ToInt32(result);
        }

        private void InsertarDetalle(
    NpgsqlConnection cn,
    NpgsqlTransaction tx,
    int idAsiento,
    AsientoDetalle item,
    int? idRelacion)
        {
            using var cmd = new NpgsqlCommand(
                AsientoDetalleQueries.Insertar, cn, tx);

            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
            cmd.Parameters.AddWithValue("@idPlanCuenta", item.IdPlanCuenta);
            cmd.Parameters.AddWithValue("@moneda", item.Moneda);
            cmd.Parameters.AddWithValue("@debe", item.Debe);
            cmd.Parameters.AddWithValue("@haber", item.Haber);

            cmd.Parameters.AddWithValue("@idTipoFacturacion",
                (object?)item.IdTipoFacturacion ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@serieComprobante",
                string.IsNullOrWhiteSpace(item.SerieComprobante)
                    ? DBNull.Value
                    : item.SerieComprobante);

            cmd.Parameters.AddWithValue("@idTercero",
                (object?)item.IdTercero ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@glosa",
                item.Glosa ?? string.Empty);

            cmd.Parameters.AddWithValue("@idRelacion",
                (object?)idRelacion ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@idTipoOperacion",
                (object?)item.IdTipoOperacion ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@idCosto",
                (object?)item.IdCosto ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }
    }
}
