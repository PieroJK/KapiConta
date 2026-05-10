using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Models.DTOs;
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

        public List<AsientoBusquedaRegistroItem> BuscarAsientos(
    int idEmpresa,
    int? idMes,
    int? idSubDiario,
    string texto)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            var lista = new List<AsientoBusquedaRegistroItem>();

            using var cmd = new NpgsqlCommand(AsientoQueries.BuscarAsientos, cn);

            cmd.Parameters.AddWithValue("@idEmpresa", idEmpresa);

            // ✅ tipado explícito para nullable integers
            var pMes = cmd.Parameters.Add("@idMes", NpgsqlTypes.NpgsqlDbType.Integer);
            pMes.Value = idMes.HasValue ? idMes.Value : DBNull.Value;

            var pSub = cmd.Parameters.Add("@idSubDiario", NpgsqlTypes.NpgsqlDbType.Integer);
            pSub.Value = idSubDiario.HasValue ? idSubDiario.Value : DBNull.Value;

            texto = texto?.Trim() ?? string.Empty;
            cmd.Parameters.AddWithValue("@texto", texto);
            cmd.Parameters.AddWithValue("@textoLike", $"%{texto}%");

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(Data.Mappings.AsientoBusquedaRegistroItemMapper.Map(dr));
            }

            return lista;
        }

        // Agrega esto en AsientoService.cs

        public AsientoCargadoItem? ObtenerAsientoPorId(int idAsiento)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            var cabecera = ObtenerCabeceraPorId(cn, idAsiento);
            if (cabecera == null)
                return null;

            cabecera.Detalles = ObtenerDetallesPorAsiento(cn, idAsiento);
            cabecera.Relaciones = ObtenerRelacionesPorAsiento(cn, idAsiento);

            return cabecera;
        }

        // =========================
        // PRIVADOS DE CARGA
        // =========================

        private AsientoCargadoItem? ObtenerCabeceraPorId(NpgsqlConnection cn, int idAsiento)
        {
            string sql = @"
        SELECT id_asiento, id_mes, id_sub_diario, id_libro,
               referencia, fecha, moneda, id_tipo_cambio, fecha_ven
        FROM asiento
        WHERE id_asiento = @idAsiento
          AND estado = true;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);

            using var dr = cmd.ExecuteReader();
            if (!dr.Read())
                return null;

            return new AsientoCargadoItem
            {
                IdAsiento = Convert.ToInt32(dr["id_asiento"]),
                IdMes = Convert.ToInt32(dr["id_mes"]),
                IdSubDiario = Convert.ToInt32(dr["id_sub_diario"]),
                IdLibro = Convert.ToInt32(dr["id_libro"]),
                Referencia = dr["referencia"]?.ToString() ?? "",
                Fecha = LeerFecha(dr["fecha"]),
                Moneda = dr["moneda"]?.ToString() ?? "PEN",
                IdTipoCambio = dr["id_tipo_cambio"] == DBNull.Value ? null : Convert.ToInt32(dr["id_tipo_cambio"]),
                FechaVencimiento = dr["fecha_ven"] == DBNull.Value ? null : LeerFecha(dr["fecha_ven"])
            };
        }

        private DateTime LeerFecha(object valor)
        {
            if (valor is DateTime dt) return dt;
            if (valor is DateOnly dateOnly) return dateOnly.ToDateTime(TimeOnly.MinValue);
            return Convert.ToDateTime(valor);
        }

        private List<AsientoDetalleCargadoItem> ObtenerDetallesPorAsiento(NpgsqlConnection cn, int idAsiento)
        {
            var lista = new List<AsientoDetalleCargadoItem>();

            string sql = @"
        SELECT
            ad.id_plan_cuenta,
            pc.codigo,
            pc.descripcion,
            ad.moneda,
            ad.debe,
            ad.haber,
            tf.id_tipo_facturacion,
            COALESCE(tf.cod, '')          AS tipo_doc_codigo,
            COALESCE(ad.serie_comprobante, '') AS documento,
            t.id_tercero,
            COALESCE(t.documento, '')     AS ruc,
            COALESCE(t.razon_social, '')  AS razon_social,
            ad.glosa,
            topa.id_tipo_operacion,
            COALESCE(topa.nombre, '')     AS tipo_operacion_nombre,
            COALESCE(aRef.referencia, '') AS asiento_referencia
        FROM asiento_detalle ad
        INNER JOIN plan_cuenta           pc   ON pc.id_plan_cuenta      = ad.id_plan_cuenta
        LEFT  JOIN tipo_facturacion      tf   ON tf.id_tipo_facturacion = ad.id_tipo_facturacion
        LEFT  JOIN tercero               t    ON t.id_tercero           = ad.id_tercero
        LEFT  JOIN tipo_operacion_asiento topa ON topa.id_tipo_operacion = ad.id_tipo_operacion
        LEFT  JOIN relacion_asiento      ra   ON ra.id_relacion         = ad.id_relacion
        LEFT  JOIN asiento               aRef ON aRef.id_asiento        = ra.asiento_relacionado
        WHERE ad.id_asiento = @idAsiento
        ORDER BY ad.id_asiento_detalle;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new AsientoDetalleCargadoItem
                {
                    IdPlanCuenta = Convert.ToInt32(dr["id_plan_cuenta"]),
                    CuentaCodigo = dr["codigo"]?.ToString() ?? "",
                    CuentaNombre = dr["descripcion"]?.ToString() ?? "",
                    Moneda = dr["moneda"]?.ToString() ?? "PEN",
                    Debe = Convert.ToDecimal(dr["debe"]),
                    Haber = Convert.ToDecimal(dr["haber"]),
                    IdTipoDocumento = dr["id_tipo_facturacion"] == DBNull.Value ? null : Convert.ToInt32(dr["id_tipo_facturacion"]),
                    TipoDocumentoCodigo = dr["tipo_doc_codigo"]?.ToString() ?? "",
                    Documento = dr["documento"]?.ToString() ?? "",
                    IdTercero = dr["id_tercero"] == DBNull.Value ? null : Convert.ToInt32(dr["id_tercero"]),
                    Ruc = dr["ruc"]?.ToString() ?? "",
                    RazonSocial = dr["razon_social"]?.ToString() ?? "",
                    Glosa = dr["glosa"]?.ToString() ?? "",
                    IdTipoOperacion = dr["id_tipo_operacion"] == DBNull.Value ? null : Convert.ToInt32(dr["id_tipo_operacion"]),
                    TipoOperacionNombre = dr["tipo_operacion_nombre"]?.ToString() ?? "",
                    AsientoReferencia = dr["asiento_referencia"]?.ToString() ?? ""
                });
            }

            return lista;
        }

        private List<AsientoRelacionItem> ObtenerRelacionesPorAsiento(NpgsqlConnection cn, int idAsiento)
        {
            var lista = new List<AsientoRelacionItem>();

            string sql = @"
        SELECT DISTINCT
            COALESCE(aRef.referencia, '')       AS asiento_referencia,
            COALESCE(tf.cod, '')                AS tipo_doc_codigo,
            COALESCE(ad.serie_comprobante, '')  AS documento,
            COALESCE(t.documento, '')           AS ruc,
            COALESCE(t.razon_social, '')        AS razon_social
        FROM asiento_detalle ad
        LEFT JOIN tipo_facturacion  tf   ON tf.id_tipo_facturacion = ad.id_tipo_facturacion
        LEFT JOIN tercero           t    ON t.id_tercero           = ad.id_tercero
        LEFT JOIN relacion_asiento  ra   ON ra.id_relacion         = ad.id_relacion
        LEFT JOIN asiento           aRef ON aRef.id_asiento        = ra.asiento_relacionado
        WHERE ad.id_asiento = @idAsiento
          AND COALESCE(aRef.referencia, '') <> '';";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new AsientoRelacionItem
                {
                    AsientoReferencia = dr["asiento_referencia"]?.ToString() ?? "",
                    TipoDocumentoCodigo = dr["tipo_doc_codigo"]?.ToString() ?? "",
                    Documento = dr["documento"]?.ToString() ?? "",
                    Ruc = dr["ruc"]?.ToString() ?? "",
                    RazonSocial = dr["razon_social"]?.ToString() ?? ""
                });
            }

            return lista;
        }

        public void ModificarAsiento(int idAsiento, Asiento cabecera, List<AsientoDetalle> detalles)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var tx = cn.BeginTransaction();

            try
            {
                // 1. Actualizar cabecera
                ActualizarCabecera(cn, tx, idAsiento, cabecera);

                // 2. Eliminar detalle y relaciones anteriores
                EliminarDetallesDelAsiento(cn, tx, idAsiento);
                EliminarRelacionesDelAsiento(cn, tx, idAsiento);

                // 3. Re-insertar detalles
                foreach (var item in detalles)
                {
                    InsertarDetalle(cn, tx, idAsiento, item, null);
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void EliminarAsiento(int idAsiento)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var tx = cn.BeginTransaction();

            try
            {
                string sql = @"
            UPDATE asiento
            SET estado = false,
                fecha_modificacion = CURRENT_TIMESTAMP
            WHERE id_asiento = @idAsiento;";

                using var cmd = new NpgsqlCommand(sql, cn, tx);
                cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
                cmd.ExecuteNonQuery();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================
        // PRIVADOS DE MODIFICACIÓN
        // =========================

        private void ActualizarCabecera(
            NpgsqlConnection cn,
            NpgsqlTransaction tx,
            int idAsiento,
            Asiento cabecera)
        {
            string sql = @"
        UPDATE asiento
        SET id_mes          = @idMes,
            id_sub_diario   = @idSubDiario,
            id_libro        = @idLibro,
            referencia      = @referencia,
            fecha           = @fecha,
            moneda          = @moneda,
            id_tipo_cambio  = @idTipoCambio,
            fecha_ven       = @fechaVen,
            id_usuario      = @idUsuario,
            fecha_modificacion = CURRENT_TIMESTAMP
        WHERE id_asiento = @idAsiento;";

            using var cmd = new NpgsqlCommand(sql, cn, tx);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
            cmd.Parameters.AddWithValue("@idMes", cabecera.IdMes);
            cmd.Parameters.AddWithValue("@idSubDiario", cabecera.IdSubDiario);
            cmd.Parameters.AddWithValue("@idLibro", cabecera.IdLibro);
            cmd.Parameters.AddWithValue("@referencia", (object?)cabecera.Referencia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha", cabecera.Fecha.Date);
            cmd.Parameters.AddWithValue("@moneda", cabecera.Moneda);
            cmd.Parameters.AddWithValue("@idTipoCambio", (object?)cabecera.IdTipoCambio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fechaVen", (object?)cabecera.FechaVen ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idUsuario", (object?)cabecera.IdUsuario ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        private void EliminarDetallesDelAsiento(NpgsqlConnection cn, NpgsqlTransaction tx, int idAsiento)
        {
            string sql = "DELETE FROM asiento_detalle WHERE id_asiento = @idAsiento;";
            using var cmd = new NpgsqlCommand(sql, cn, tx);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
            cmd.ExecuteNonQuery();
        }

        private void EliminarRelacionesDelAsiento(NpgsqlConnection cn, NpgsqlTransaction tx, int idAsiento)
        {
            string sql = "DELETE FROM relacion_asiento WHERE asiento_origen = @idAsiento;";
            using var cmd = new NpgsqlCommand(sql, cn, tx);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
            cmd.ExecuteNonQuery();
        }
    }
}
