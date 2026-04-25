using ClosedXML.Excel;
using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models.DTOs;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Inmobiliaria_KapiConta.Services
{
    public class ImportarAsientoService
    {
        private int _correlativoActual = 0;

        // =========================
        // ENTRADA PRINCIPAL
        // =========================

        public ImportacionResult ImportarCompras(string rutaExcel, int idMes, bool esInicio)
        {
            var result = new ImportacionResult();

            try
            {
                // 1. Leer Excel
                var filas = LeerExcelCompras(rutaExcel);
                if (!filas.Any())
                    throw new Exception("El archivo Excel no tiene datos.");

                // 2. Cargar catálogos en memoria (evita N+1)
                var catalogos = CargarCatalogos();

                // 3. Validar todas las filas antes de insertar
                var errores = ValidarCompras(filas, catalogos);
                if (errores.Any())
                {
                    result.Exitoso = false;
                    result.Errores = errores;
                    return result;
                }

                // 4. Si modo inicio, desactivar asientos anteriores
                int idSubDiario = ObtenerIdSubDiario("C", catalogos);  // sub_diario.diario
                int idLibro = ObtenerIdLibro("08", catalogos);     // libro.cod

                if (esInicio)
                    DesactivarAsientos(idSubDiario, idMes);

                // 5. Inicializar correlativo
                InicializarCorrelativo("C", idMes, idSubDiario, esInicio);

                // 6. Importar cada fila
                foreach (var fila in filas)
                {
                    ImportarFilaCompra(fila, idMes, idSubDiario, idLibro, catalogos);
                    result.AsientosImportados++;
                }

                result.Exitoso = true;
            }
            catch (Exception ex)
            {
                result.Exitoso = false;
                result.Errores.Add(ex.Message);
            }

            return result;
        }

        public ImportacionResult ImportarVentas(string rutaExcel, int idMes, bool esInicio)
        {
            var result = new ImportacionResult();

            try
            {
                var filas = LeerExcelVentas(rutaExcel);
                if (!filas.Any())
                    throw new Exception("El archivo Excel no tiene datos.");

                var catalogos = CargarCatalogos();

                var errores = ValidarVentas(filas, catalogos);
                if (errores.Any())
                {
                    result.Exitoso = false;
                    result.Errores = errores;
                    return result;
                }

                int idSubDiario = ObtenerIdSubDiario("V", catalogos);
                int idLibro = ObtenerIdLibro("14", catalogos);

                if (esInicio)
                    DesactivarAsientos(idSubDiario, idMes);

                InicializarCorrelativo("V", idMes, idSubDiario, esInicio);

                foreach (var fila in filas)
                {
                    ImportarFilaVenta(fila, idMes, idSubDiario, idLibro, catalogos);
                    result.AsientosImportados++;
                }

                result.Exitoso = true;
            }
            catch (Exception ex)
            {
                result.Exitoso = false;
                result.Errores.Add(ex.Message);
            }

            return result;
        }

        // =========================
        // LEER EXCEL
        // =========================

        private List<CompraExcelFila> LeerExcelCompras(string ruta)
        {
            var lista = new List<CompraExcelFila>();

            using var wb = new XLWorkbook(ruta);
            var ws = wb.Worksheet(1);
            var filas = ws.RangeUsed().RowsUsed().Skip(1).ToList();

            if (filas.Any())
            {
                var primeraFila = filas.First();
                var debug = "";
                for (int i = 1; i <= 38; i++)
                    debug += $"Col {i}: '{primeraFila.Cell(i).GetString()}'\n";
                MessageBox.Show(debug);
            }

            foreach (var row in filas)
            {
                lista.Add(new CompraExcelFila
                {
                    Fecha = row.Cell(1).GetValue<DateTime>(),
                    FechaVcmto = row.Cell(2).TryGetValue<DateTime>(out var fv) ? fv : null,
                    TipoDoc = row.Cell(3).GetString().Trim(),
                    Serie = row.Cell(4).GetString().Trim(),
                    Documento = row.Cell(5).GetString().Trim(),
                    Ident = row.Cell(6).GetString().Trim(),
                    Ruc = row.Cell(7).GetString().Trim(),
                    Nombre = row.Cell(8).GetString().Trim(),
                    Costo = row.Cell(9).GetString().Trim(),
                    Glosa = row.Cell(10).GetString().Trim(),
                    Exonerado = row.Cell(11).TryGetValue<decimal>(out var ex1) ? ex1 : 0,
                    Inafecto = row.Cell(12).TryGetValue<decimal>(out var in1) ? in1 : 0,
                    Imponible1 = row.Cell(13).TryGetValue<decimal>(out var im1) ? im1 : 0,
                    Igv1 = row.Cell(14).TryGetValue<decimal>(out var ig1) ? ig1 : 0,
                    Imponible2 = row.Cell(15).TryGetValue<decimal>(out var im2) ? im2 : 0,
                    Igv2 = row.Cell(16).TryGetValue<decimal>(out var ig2) ? ig2 : 0,
                    Imponible3 = row.Cell(17).TryGetValue<decimal>(out var im3) ? im3 : 0,
                    Igv3 = row.Cell(18).TryGetValue<decimal>(out var ig3) ? ig3 : 0,
                    Imponible4 = row.Cell(19).TryGetValue<decimal>(out var im4) ? im4 : 0,
                    Igv4 = row.Cell(20).TryGetValue<decimal>(out var ig4) ? ig4 : 0,
                    Imponible5 = row.Cell(21).TryGetValue<decimal>(out var im5) ? im5 : 0,
                    Igv5 = row.Cell(22).TryGetValue<decimal>(out var ig5) ? ig5 : 0,
                    Total = row.Cell(23).TryGetValue<decimal>(out var tot) ? tot : 0,
                    Moneda = string.IsNullOrWhiteSpace(row.Cell(24).GetString()) ? "PEN" : row.Cell(24).GetString().Trim(),
                    TipoCambio = row.Cell(25).TryGetValue<decimal>(out var tc) ? tc : 1,
                    CtaExonerada = row.Cell(26).GetString().Trim(),
                    CtaInafecta = row.Cell(27).GetString().Trim(),
                    CtaImponible1 = row.Cell(28).GetString().Trim(),
                    CtaIgv1 = row.Cell(29).GetString().Trim(),
                    CtaImponible2 = row.Cell(30).GetString().Trim(),
                    CtaIgv2 = row.Cell(31).GetString().Trim(),
                    CtaImponible3 = row.Cell(32).GetString().Trim(),
                    CtaIgv3 = row.Cell(33).GetString().Trim(),
                    CtaImponible4 = row.Cell(34).GetString().Trim(),
                    CtaIgv4 = row.Cell(35).GetString().Trim(),
                    CtaImponible5 = row.Cell(36).GetString().Trim(),
                    CtaIgv5 = row.Cell(37).GetString().Trim(),
                    CtaTotal = row.Cell(38).GetString().Trim()
                });
            }

            return lista;
        }

        private List<VentaExcelFila> LeerExcelVentas(string ruta)
        {
            var lista = new List<VentaExcelFila>();

            using var wb = new XLWorkbook(ruta);
            var ws = wb.Worksheet(1);
            var filas = ws.RangeUsed().RowsUsed().Skip(1);

            foreach (var row in filas)
            {
                lista.Add(new VentaExcelFila
                {
                    Fecha = row.Cell(1).GetValue<DateTime>(),
                    FechaVcmto = row.Cell(2).TryGetValue<DateTime>(out var fv) ? fv : null,
                    TipoDoc = row.Cell(3).GetString().Trim(),
                    Serie = row.Cell(4).GetString().Trim(),
                    Documento = row.Cell(5).GetString().Trim(),
                    Ident = row.Cell(6).GetString().Trim(),
                    Ruc = row.Cell(7).GetString().Trim(),
                    Nombre = row.Cell(8).GetString().Trim(),
                    Costo = row.Cell(9).GetString().Trim(),
                    Glosa = row.Cell(10).GetString().Trim(),
                    Exonerado = row.Cell(11).TryGetValue<decimal>(out var ex1) ? ex1 : 0,
                    Inafecto = row.Cell(12).TryGetValue<decimal>(out var in1) ? in1 : 0,
                    Imponible1 = row.Cell(13).TryGetValue<decimal>(out var im1) ? im1 : 0,
                    Imponible2 = row.Cell(14).TryGetValue<decimal>(out var im2) ? im2 : 0,
                    Imponible3 = row.Cell(15).TryGetValue<decimal>(out var im3) ? im3 : 0,
                    Imponible4 = row.Cell(16).TryGetValue<decimal>(out var im4) ? im4 : 0,
                    Imponible5 = row.Cell(17).TryGetValue<decimal>(out var im5) ? im5 : 0,
                    Igv = row.Cell(18).TryGetValue<decimal>(out var ig) ? ig : 0,
                    Total = row.Cell(19).TryGetValue<decimal>(out var tot) ? tot : 0,
                    Moneda = string.IsNullOrWhiteSpace(row.Cell(20).GetString()) ? "PEN" : row.Cell(20).GetString().Trim(),
                    TipoCambio = row.Cell(21).TryGetValue<decimal>(out var tc) ? tc : 1,
                    CtaExonerada = row.Cell(22).GetString().Trim(),
                    CtaInafecta = row.Cell(23).GetString().Trim(),
                    CtaImponible1 = row.Cell(24).GetString().Trim(),
                    CtaImponible2 = row.Cell(25).GetString().Trim(),
                    CtaImponible3 = row.Cell(26).GetString().Trim(),
                    CtaImponible4 = row.Cell(27).GetString().Trim(),
                    CtaImponible5 = row.Cell(28).GetString().Trim(),
                    CtaIgv = row.Cell(29).GetString().Trim(),
                    CtaTotal = row.Cell(30).GetString().Trim()
                });
            }

            return lista;
        }

        // =========================
        // CATÁLOGOS EN MEMORIA
        // =========================

        private CatalogosImport CargarCatalogos()
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();
            var cat = new CatalogosImport();

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarCuentas, cn))
            {
                cmd.Parameters.AddWithValue("@id", Session.CurrentEmpresa.IdEmpresa);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.Cuentas[dr["codigo"].ToString()!] = Convert.ToInt32(dr["id_plan_cuenta"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarTerceros, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.Terceros[dr["documento"].ToString()!] = Convert.ToInt32(dr["id_tercero"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarTiposFacturacion, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.TiposFacturacion[dr["cod"].ToString()!] = Convert.ToInt32(dr["id_tipo_facturacion"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarTiposDocTercero, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.TiposDocTercero[Convert.ToInt32(dr["cod"])] = Convert.ToInt32(dr["id_tercero_tipo_documento"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarSubDiarios, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.SubDiarios[dr["diario"].ToString()!] = Convert.ToInt32(dr["id_sub_diario"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarLibros, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.Libros[dr["cod"].ToString()!] = Convert.ToInt32(dr["id_libro"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarCostos, cn))
            {
                cmd.Parameters.AddWithValue("@id", Session.CurrentEmpresa.IdEmpresa);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.Costos[dr["descripcion"].ToString()!] = Convert.ToInt32(dr["id_costo"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarMeses, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.Meses[dr["mes"].ToString()!] = Convert.ToInt32(dr["id_mes"]);
            }

            using (var cmd = new NpgsqlCommand(ImportarAsientoQueries.CargarTiposOperacion, cn))
            {
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                    cat.TiposOperacion[dr["codigo"].ToString()!] = Convert.ToInt32(dr["id_tipo_operacion"]);
            }

            return cat;
        }

        // =========================
        // VALIDACIONES
        // =========================

        private List<string> ValidarCompras(List<CompraExcelFila> filas, CatalogosImport cat)
        {
            var errores = new List<string>();

            for (int i = 0; i < filas.Count; i++)
            {
                var f = filas[i];
                int nro = i + 2;

                // ✅ Solo validar si la cuenta tiene valor real (no vacío ni "0")
                if (EsCuentaValida(f.CtaExonerada) && !cat.Cuentas.ContainsKey(f.CtaExonerada))
                    errores.Add($"Fila {nro}: cuenta exonerada '{f.CtaExonerada}' no existe.");

                if (EsCuentaValida(f.CtaInafecta) && !cat.Cuentas.ContainsKey(f.CtaInafecta))
                    errores.Add($"Fila {nro}: cuenta inafecta '{f.CtaInafecta}' no existe.");

                if (EsCuentaValida(f.CtaImponible1) && !cat.Cuentas.ContainsKey(f.CtaImponible1))
                    errores.Add($"Fila {nro}: cuenta imponible 1 '{f.CtaImponible1}' no existe.");

                if (EsCuentaValida(f.CtaIgv1) && !cat.Cuentas.ContainsKey(f.CtaIgv1))
                    errores.Add($"Fila {nro}: cuenta IGV 1 '{f.CtaIgv1}' no existe.");

                if (EsCuentaValida(f.CtaImponible2) && !cat.Cuentas.ContainsKey(f.CtaImponible2))
                    errores.Add($"Fila {nro}: cuenta imponible 2 '{f.CtaImponible2}' no existe.");

                if (EsCuentaValida(f.CtaIgv2) && !cat.Cuentas.ContainsKey(f.CtaIgv2))
                    errores.Add($"Fila {nro}: cuenta IGV 2 '{f.CtaIgv2}' no existe.");

                if (EsCuentaValida(f.CtaTotal) && !cat.Cuentas.ContainsKey(f.CtaTotal))
                    errores.Add($"Fila {nro}: cuenta total '{f.CtaTotal}' no existe.");

                if (!string.IsNullOrWhiteSpace(f.TipoDoc) && !cat.TiposFacturacion.ContainsKey(f.TipoDoc))
                    errores.Add($"Fila {nro}: tipo documento '{f.TipoDoc}' no existe.");

                if (string.IsNullOrWhiteSpace(f.Glosa))
                    errores.Add($"Fila {nro}: glosa vacía.");
            }

            return errores;
        }

        private bool EsCuentaValida(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return false;
            if (valor.Trim() == "0") return false;
            if (valor.Trim() == "0.00") return false;
            // ✅ Si es puramente numérico con decimales, no es una cuenta
            if (decimal.TryParse(valor, out _) && !valor.Contains('.') == false) return false;
            return true;
        }

        private List<string> ValidarVentas(List<VentaExcelFila> filas, CatalogosImport cat)
        {
            var errores = new List<string>();

            for (int i = 0; i < filas.Count; i++)
            {
                var f = filas[i];
                int nro = i + 2;

                if (!string.IsNullOrWhiteSpace(f.CtaExonerada) && !cat.Cuentas.ContainsKey(f.CtaExonerada))
                    errores.Add($"Fila {nro}: cuenta exonerada '{f.CtaExonerada}' no existe.");

                if (!string.IsNullOrWhiteSpace(f.CtaInafecta) && !cat.Cuentas.ContainsKey(f.CtaInafecta))
                    errores.Add($"Fila {nro}: cuenta inafecta '{f.CtaInafecta}' no existe.");

                if (!string.IsNullOrWhiteSpace(f.CtaTotal) && !cat.Cuentas.ContainsKey(f.CtaTotal))
                    errores.Add($"Fila {nro}: cuenta total '{f.CtaTotal}' no existe.");

                if (!string.IsNullOrWhiteSpace(f.TipoDoc) && !cat.TiposFacturacion.ContainsKey(f.TipoDoc))
                    errores.Add($"Fila {nro}: tipo documento '{f.TipoDoc}' no existe.");

                if (string.IsNullOrWhiteSpace(f.Glosa))
                    errores.Add($"Fila {nro}: glosa vacía.");
            }

            return errores;
        }

        // =========================
        // IMPORTAR FILA
        // =========================

        private void ImportarFilaCompra(
    CompraExcelFila fila,
    int idMes, int idSubDiario, int idLibro,
    CatalogosImport cat)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();
            using var tx = cn.BeginTransaction();

            try
            {
                int? idTipoFacturacion = ResolverTipoFacturacion(fila.TipoDoc, cat);
                int? idTercero = ResolverOCrearTercero(fila.Ruc, fila.Nombre, fila.Ident, cat, cn, tx);
                int? idCosto = ResolverCosto(fila.Costo, cat, cn, tx);
                int? idTipoCambio = ResolverTipoCambio(fila.Fecha, fila.TipoCambio, cn, tx);
                string referencia = GenerarCorrelativo("C", idMes);
                string serieDoc = $"{fila.Serie}-{fila.Documento}";

                int idAsiento = InsertarCabecera(cn, tx,
                    idMes, idSubDiario, idLibro,
                    referencia, fila.Fecha, fila.FechaVcmto,
                    fila.Moneda, idTipoCambio);

                // ✅ EXONERADO → DEBE
                if (fila.Exonerado != 0 && EsCuentaValida(fila.CtaExonerada))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaExonerada],
                        fila.Exonerado, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("EXO", cat));

                // ✅ INAFECTO → DEBE
                if (fila.Inafecto != 0 && EsCuentaValida(fila.CtaInafecta))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaInafecta],
                        fila.Inafecto, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("INA", cat));

                // ✅ IMPONIBLE 1 → DEBE
                if (fila.Imponible1 != 0 && EsCuentaValida(fila.CtaImponible1))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible1],
                        fila.Imponible1, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA1", cat));

                // ✅ IGV 1 → DEBE
                if (fila.Igv1 != 0 && EsCuentaValida(fila.CtaIgv1))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv1],
                        fila.Igv1, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV1", cat));

                // ✅ IMPONIBLE 2 → DEBE
                if (fila.Imponible2 != 0 && EsCuentaValida(fila.CtaImponible2))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible2],
                        fila.Imponible2, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA2", cat));

                // ✅ IGV 2 → DEBE
                if (fila.Igv2 != 0 && EsCuentaValida(fila.CtaIgv2))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv2],
                        fila.Igv2, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV2", cat));

                // ✅ IMPONIBLE 3 → DEBE
                if (fila.Imponible3 != 0 && EsCuentaValida(fila.CtaImponible3))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible3],
                        fila.Imponible3, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA3", cat));

                // ✅ IGV 3 → DEBE
                if (fila.Igv3 != 0 && EsCuentaValida(fila.CtaIgv3))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv3],
                        fila.Igv3, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV3", cat));

                // ✅ IMPONIBLE 4 → DEBE
                if (fila.Imponible4 != 0 && EsCuentaValida(fila.CtaImponible4))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible4],
                        fila.Imponible4, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA4", cat));

                // ✅ IGV 4 → DEBE
                if (fila.Igv4 != 0 && EsCuentaValida(fila.CtaIgv4))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv4],
                        fila.Igv4, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV4", cat));

                // ✅ IMPONIBLE 5 → DEBE
                if (fila.Imponible5 != 0 && EsCuentaValida(fila.CtaImponible5))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible5],
                        fila.Imponible5, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA5", cat));

                // ✅ IGV 5 → DEBE
                if (fila.Igv5 != 0 && EsCuentaValida(fila.CtaIgv5))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv5],
                        fila.Igv5, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV5", cat));

                // ✅ TOTAL → HABER (cuenta proveedores)
                if (fila.Total != 0 && EsCuentaValida(fila.CtaTotal))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaTotal],
                        0, fila.Total, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        null);

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private void ImportarFilaVenta(
    VentaExcelFila fila,
    int idMes, int idSubDiario, int idLibro,
    CatalogosImport cat)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();
            using var tx = cn.BeginTransaction();

            try
            {
                int? idTipoFacturacion = ResolverTipoFacturacion(fila.TipoDoc, cat);
                int? idTercero = ResolverOCrearTercero(fila.Ruc, fila.Nombre, fila.Ident, cat, cn, tx);
                int? idCosto = ResolverCosto(fila.Costo, cat, cn, tx);
                int? idTipoCambio = ResolverTipoCambio(fila.Fecha, fila.TipoCambio, cn, tx);
                string referencia = GenerarCorrelativo("V", idMes);
                string serieDoc = $"{fila.Serie}-{fila.Documento}";

                int idAsiento = InsertarCabecera(cn, tx,
                    idMes, idSubDiario, idLibro,
                    referencia, fila.Fecha, fila.FechaVcmto,
                    fila.Moneda, idTipoCambio);

                // ✅ TOTAL → DEBE (cuenta clientes)
                if (fila.Total != 0 && EsCuentaValida(fila.CtaTotal))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaTotal],
                        fila.Total, 0, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto, null);

                // ✅ EXONERADO → HABER
                if (fila.Exonerado != 0 && EsCuentaValida(fila.CtaExonerada))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaExonerada],
                        0, fila.Exonerado, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("EXO", cat));

                // ✅ INAFECTO → HABER
                if (fila.Inafecto != 0 && EsCuentaValida(fila.CtaInafecta))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaInafecta],
                        0, fila.Inafecto, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("INA", cat));

                // ✅ IMPONIBLE 1 → HABER
                if (fila.Imponible1 != 0 && EsCuentaValida(fila.CtaImponible1))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible1],
                        0, fila.Imponible1, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA1", cat));

                // ✅ IMPONIBLE 2 → HABER
                if (fila.Imponible2 != 0 && EsCuentaValida(fila.CtaImponible2))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible2],
                        0, fila.Imponible2, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA2", cat));

                // ✅ IMPONIBLE 3 → HABER
                if (fila.Imponible3 != 0 && EsCuentaValida(fila.CtaImponible3))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible3],
                        0, fila.Imponible3, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA3", cat));

                // ✅ IMPONIBLE 4 → HABER
                if (fila.Imponible4 != 0 && EsCuentaValida(fila.CtaImponible4))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible4],
                        0, fila.Imponible4, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA4", cat));

                // ✅ IMPONIBLE 5 → HABER
                if (fila.Imponible5 != 0 && EsCuentaValida(fila.CtaImponible5))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaImponible5],
                        0, fila.Imponible5, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("GRA5", cat));

                // ✅ IGV → HABER
                if (fila.Igv != 0 && EsCuentaValida(fila.CtaIgv))
                    InsertarDetalle(cn, tx, idAsiento, cat.Cuentas[fila.CtaIgv],
                        0, fila.Igv, fila.Moneda, fila.Glosa,
                        idTipoFacturacion, serieDoc, idTercero, null, idCosto,
                        ResolverTipoOperacion("IGV1", cat));

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // =========================
        // HELPERS BD
        // =========================
        // Nuevo método helper:
        private int? ResolverTipoOperacion(string codigo, CatalogosImport cat)
        {
            if (string.IsNullOrWhiteSpace(codigo)) return null;
            return cat.TiposOperacion.TryGetValue(codigo, out int id) ? id : null;
        }
        private int InsertarCabecera(
    NpgsqlConnection cn, NpgsqlTransaction tx,
    int idMes, int idSubDiario, int idLibro,
    string referencia, DateTime fecha, DateTime? fechaVen,
    string moneda, int? idTipoCambio)
        {
            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.InsertarCabecera, cn, tx);
            cmd.Parameters.AddWithValue("@idEmpresa", Session.CurrentEmpresa.IdEmpresa);
            cmd.Parameters.AddWithValue("@idPeriodo", Session.CurrentPeriodo.IdPeriodo);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@idSubDiario", idSubDiario);
            cmd.Parameters.AddWithValue("@idLibro", idLibro);
            cmd.Parameters.AddWithValue("@referencia", referencia);
            cmd.Parameters.AddWithValue("@fecha", fecha.Date);
            cmd.Parameters.AddWithValue("@fechaVen", fechaVen.HasValue ? fechaVen.Value.Date : DBNull.Value);
            cmd.Parameters.AddWithValue("@moneda", moneda);
            cmd.Parameters.AddWithValue("@idTipoCambio", idTipoCambio.HasValue ? idTipoCambio.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@idUsuario", Session.CurrentUser.Id);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private void InsertarDetalle(
    NpgsqlConnection cn, NpgsqlTransaction tx,
    int idAsiento, int idCuenta,
    decimal debe, decimal haber,
    string moneda, string glosa,
    int? idTipoFacturacion, string? serieDoc,
    int? idTercero, int? idRelacion, int? idCosto,
    int? idTipoOperacion)
        {
            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.InsertarDetalle, cn, tx);
            cmd.Parameters.AddWithValue("@idAsiento", idAsiento);
            cmd.Parameters.AddWithValue("@idCuenta", idCuenta);
            cmd.Parameters.AddWithValue("@moneda", moneda);
            cmd.Parameters.AddWithValue("@debe", debe);
            cmd.Parameters.AddWithValue("@haber", haber);
            cmd.Parameters.AddWithValue("@idTipoFacturacion", idTipoFacturacion.HasValue ? idTipoFacturacion.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@serieDoc", string.IsNullOrWhiteSpace(serieDoc) ? DBNull.Value : serieDoc);
            cmd.Parameters.AddWithValue("@idTercero", idTercero.HasValue ? idTercero.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@glosa", glosa ?? "");
            cmd.Parameters.AddWithValue("@idRelacion", idRelacion.HasValue ? idRelacion.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@idCosto", idCosto.HasValue ? idCosto.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@idTipoOperacion", idTipoOperacion.HasValue ? idTipoOperacion.Value : DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        private int ResolverOCrearTercero(
            string ruc, string nombre, string ident,
            CatalogosImport cat,
            NpgsqlConnection cn, NpgsqlTransaction tx)
        {
            if (string.IsNullOrWhiteSpace(ruc)) return 0;

            // ✅ Ya existe en memoria
            if (cat.Terceros.TryGetValue(ruc, out int idExistente))
                return idExistente;

            // ✅ Crear nuevo tercero
            int codTipoDoc = ruc.Length == 11 ? 6 : ruc.Length == 8 ? 1
                : throw new Exception($"Documento inválido: {ruc}");

            if (!cat.TiposDocTercero.TryGetValue(codTipoDoc, out int idTipoDocTercero))
                throw new Exception($"No existe tipo documento con código {codTipoDoc}");

            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.InsertarTercero, cn, tx);
            cmd.Parameters.AddWithValue("@doc", ruc);
            cmd.Parameters.AddWithValue("@razon", nombre ?? "");
            cmd.Parameters.AddWithValue("@tipo", idTipoDocTercero);

            int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());

            // ✅ Agregar al catálogo en memoria para siguiente fila
            cat.Terceros[ruc] = nuevoId;

            return nuevoId;
        }

        private int? ResolverTipoFacturacion(string cod, CatalogosImport cat)
        {
            if (string.IsNullOrWhiteSpace(cod)) return null;
            return cat.TiposFacturacion.TryGetValue(cod, out int id) ? id : null;
        }

        private int? ResolverCosto(
     string descripcion,
     CatalogosImport cat,
     NpgsqlConnection cn,
     NpgsqlTransaction tx)
        {
            // ✅ Si está vacío → null en asiento_detalle
            if (string.IsNullOrWhiteSpace(descripcion)) return null;

            // ✅ Ya existe en memoria por descripción
            if (cat.Costos.TryGetValue(descripcion.Trim(), out int idExistente))
                return idExistente;

            // ✅ Crear nuevo costo
            // El código se genera automáticamente como secuencia
            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.InsertarCosto, cn, tx);
            cmd.Parameters.AddWithValue("@idEmpresa", Session.CurrentEmpresa.IdEmpresa);
            cmd.Parameters.AddWithValue("@descripcion", descripcion.Trim());

            // ✅ El código = id_costo, lo obtenemos después del insert
            // Primero insertamos con código temporal
            cmd.Parameters.AddWithValue("@codigo", descripcion.Trim());

            int nuevoId = Convert.ToInt32(cmd.ExecuteScalar());

            // ✅ Actualizar el código con el id generado
            using var cmdUpdate = new NpgsqlCommand(ImportarAsientoQueries.ActualizarCodigoCosto, cn, tx);
            cmdUpdate.Parameters.AddWithValue("@codigo", nuevoId.ToString());
            cmdUpdate.Parameters.AddWithValue("@id", nuevoId);
            cmdUpdate.ExecuteNonQuery();

            // ✅ Agregar al catálogo en memoria
            cat.Costos[descripcion.Trim()] = nuevoId;

            return nuevoId;
        }

        private int? ResolverTipoCambio(DateTime fecha, decimal tipoCambio, NpgsqlConnection cn, NpgsqlTransaction tx)
        {
            if (tipoCambio <= 1) return null;

            // Buscar si ya existe tipo cambio para esa fecha
            using var cmdBuscar = new NpgsqlCommand(ImportarAsientoQueries.BuscarTipoCambio, cn, tx);
            // ...
            cmdBuscar.Parameters.AddWithValue("@fecha", fecha.Date);
            var result = cmdBuscar.ExecuteScalar();
            if (result != null) return Convert.ToInt32(result);

            // Crear nuevo tipo cambio
            using var cmdInsert = new NpgsqlCommand(ImportarAsientoQueries.InsertarTipoCambio, cn, tx);
            cmdInsert.Parameters.AddWithValue("@fecha", fecha.Date);
            cmdInsert.Parameters.AddWithValue("@tc", tipoCambio);

            return Convert.ToInt32(cmdInsert.ExecuteScalar());
        }

        private int ObtenerIdSubDiario(string diario, CatalogosImport cat)
        {
            if (!cat.SubDiarios.TryGetValue(diario, out int id))
                throw new Exception($"No existe subdiario con código {diario}");
            return id;
        }

        private int ObtenerIdLibro(string cod, CatalogosImport cat)
        {
            if (!cat.Libros.TryGetValue(cod, out int id))
                throw new Exception($"No existe libro con código {cod}");
            return id;
        }

        private void DesactivarAsientos(int idSubDiario, int idMes)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.DesactivarAsientos, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", Session.CurrentEmpresa.IdEmpresa);
            cmd.Parameters.AddWithValue("@idPeriodo", Session.CurrentPeriodo.IdPeriodo);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@idSubDiario", idSubDiario);

            cmd.ExecuteNonQuery();
        }

        private void InicializarCorrelativo(string tipo, int idMes, int idSubDiario, bool esInicio)
        {
            _correlativoActual = 0;

            if (esInicio)
            {
                _correlativoActual = 1;
                return;
            }

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(ImportarAsientoQueries.ObtenerUltimaReferencia, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", Session.CurrentEmpresa.IdEmpresa);
            cmd.Parameters.AddWithValue("@idPeriodo", Session.CurrentPeriodo.IdPeriodo);
            cmd.Parameters.AddWithValue("@idMes", idMes);
            cmd.Parameters.AddWithValue("@idSubDiario", idSubDiario);

            var ultimaRef = cmd.ExecuteScalar()?.ToString();

            if (!string.IsNullOrWhiteSpace(ultimaRef) && ultimaRef.Length >= 9)
            {
                string numero = ultimaRef.Substring(3);
                if (int.TryParse(numero, out int n))
                    _correlativoActual = n + 1;
                else
                    _correlativoActual = 1;
            }
            else
            {
                _correlativoActual = 1;
            }
        }

        private string GenerarCorrelativo(string tipo, int idMes)
        {
            // Obtener código del mes
            string codigoMes = idMes.ToString("D2");

            string correlativo = _correlativoActual.ToString("D6");
            _correlativoActual++;

            return $"{tipo}{codigoMes}{correlativo}";
        }
    }

    // =========================
    // CATÁLOGOS EN MEMORIA
    // =========================

    public class CatalogosImport
    {
        public Dictionary<string, int> Cuentas { get; } = new();
        public Dictionary<string, int> Terceros { get; } = new();
        public Dictionary<string, int> TiposFacturacion { get; } = new();
        public Dictionary<int, int> TiposDocTercero { get; } = new();
        public Dictionary<string, int> SubDiarios { get; } = new();
        public Dictionary<string, int> Libros { get; } = new();
        public Dictionary<string, int> Costos { get; } = new();
        public Dictionary<string, int> Meses { get; } = new();

        // En CatalogosImport agrega:
        public Dictionary<string, int> TiposOperacion { get; } = new();
    }
}