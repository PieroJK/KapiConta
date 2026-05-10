// ViewModels/ConsultaBalanceViewModel.cs
using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models.DTOs;
using Npgsql;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ConsultaBalanceViewModel : INotifyPropertyChanged
    {
        // ─── INotifyPropertyChanged ───────────────────────────────────
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // ─── Comandos ─────────────────────────────────────────────────
        public ICommand ConsultarCommand { get; private set; }
        public ICommand ExcelCommand { get; private set; }
        public ICommand VolverCommand { get; private set; }

        // ─── Colecciones ──────────────────────────────────────────────
        public ObservableCollection<MesFiltroItem> Meses { get; } = new()
        {
            new MesFiltroItem(1,  "Apertura"),
            new MesFiltroItem(2,  "Enero"),
            new MesFiltroItem(3,  "Febrero"),
            new MesFiltroItem(4,  "Marzo"),
            new MesFiltroItem(5,  "Abril"),
            new MesFiltroItem(6,  "Mayo"),
            new MesFiltroItem(7,  "Junio"),
            new MesFiltroItem(8,  "Julio"),
            new MesFiltroItem(9,  "Agosto"),
            new MesFiltroItem(10, "Septiembre"),
            new MesFiltroItem(11, "Octubre"),
            new MesFiltroItem(12, "Noviembre"),
            new MesFiltroItem(13, "Diciembre"),
            new MesFiltroItem(14, "Cierre")
        };

        public ObservableCollection<BalanceDto> Resultados { get; } = new();

        // ─── Propiedades ──────────────────────────────────────────────

        private MesFiltroItem _mesInicioSeleccionado;
        public MesFiltroItem MesInicioSeleccionado
        {
            get => _mesInicioSeleccionado;
            set { _mesInicioSeleccionado = value; OnPropertyChanged(); }
        }

        private MesFiltroItem _mesFinSeleccionado;
        public MesFiltroItem MesFinSeleccionado
        {
            get => _mesFinSeleccionado;
            set { _mesFinSeleccionado = value; OnPropertyChanged(); }
        }

        // ─── Constructor ──────────────────────────────────────────────
        public ConsultaBalanceViewModel()
        {
            ConsultarCommand = new RelayCommand(Consultar, PuedeConsultar);
            ExcelCommand = new RelayCommand(Exportar, () => Resultados.Any());
            VolverCommand = new RelayCommand(Volver);

            // Selección inicial: Apertura → Cierre (igual que el original: "00" → "14")
            _mesInicioSeleccionado = Meses.First();
            _mesFinSeleccionado = Meses.Last();

            // Carga inicial automática, igual que el CargarDatos() del constructor original
            Consultar();
        }

        // ─── Métodos privados ─────────────────────────────────────────

        private bool PuedeConsultar()
            => MesInicioSeleccionado != null && MesFinSeleccionado != null;

        private void Consultar()
        {
            if (!PuedeConsultar())
            {
                MessageBox.Show(
                    "Por favor, seleccione un mes de inicio y un mes de fin.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Resultados.Clear();

                using var conn = DbConnectionFactory.Create();
                conn.Open();

                // Query idéntica al original — sin tocar la lógica SQL
                const string query = @"
                    SELECT 
                        p.codigo, 
                        p.descripcion AS cuenta,
                        COALESCE(SUM(ad.debe),  0) AS debe,
                        COALESCE(SUM(ad.haber), 0) AS haber,

                        CASE 
                            WHEN SUM(ad.debe) > SUM(ad.haber) THEN SUM(ad.debe - ad.haber)
                            ELSE 0 
                        END AS deudor,

                        CASE 
                            WHEN SUM(ad.haber) > SUM(ad.debe) THEN SUM(ad.haber - ad.debe)
                            ELSE 0 
                        END AS acreedor,

                        CASE 
                            WHEN p.codigo LIKE '0%' OR p.codigo LIKE '5%' THEN SUM(ad.debe)
                            ELSE 0 
                        END AS activo,

                        CASE 
                            WHEN p.codigo LIKE '0%' OR p.codigo LIKE '5%' THEN SUM(ad.haber)
                            ELSE 0 
                        END AS pasivo,

                        CASE 
                            WHEN p.codigo LIKE '6%' OR p.codigo LIKE '7%'
                              OR p.codigo LIKE '8%' OR p.codigo LIKE '9%' THEN SUM(ad.debe)
                            ELSE 0 
                        END AS resultado_debe,

                        CASE 
                            WHEN p.codigo LIKE '6%' OR p.codigo LIKE '7%'
                              OR p.codigo LIKE '8%' OR p.codigo LIKE '9%' THEN SUM(ad.haber)
                            ELSE 0 
                        END AS resultado_haber

                    FROM plan_cuenta p
                    LEFT JOIN asiento_detalle ad ON p.id_plan_cuenta  = ad.id_plan_cuenta
                    LEFT JOIN asiento         a  ON a.id_asiento      = ad.id_asiento
                    WHERE a.id_mes    BETWEEN @mesInicio AND @mesFin
                      AND a.id_empresa = @empresa
                    GROUP BY p.codigo, p.descripcion
                    ORDER BY p.codigo;";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mesInicio", MesInicioSeleccionado.Id);
                cmd.Parameters.AddWithValue("@mesFin", MesFinSeleccionado.Id);
                cmd.Parameters.AddWithValue("@empresa", Session.CurrentEmpresa.IdEmpresa);

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Resultados.Add(new BalanceDto
                    {
                        Codigo = dr["codigo"].ToString(),
                        Cuenta = dr["cuenta"].ToString(),
                        Debe = (decimal)dr["debe"],
                        Haber = (decimal)dr["haber"],
                        Deudor = (decimal)dr["deudor"],
                        Acreedor = (decimal)dr["acreedor"],
                        Activo = (decimal)dr["activo"],
                        Pasivo = (decimal)dr["pasivo"],
                        ResultadoDebe = (decimal)dr["resultado_debe"],
                        ResultadoHaber = (decimal)dr["resultado_haber"]
                    });
                }

                ((RelayCommand)ExcelCommand).RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);
            }
        }

        private void Exportar()
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Leonardo");

                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Detalle");

                // Encabezados
                ws.Cells[1, 1].Value = "Código";
                ws.Cells[1, 2].Value = "Cuenta";
                ws.Cells[1, 3].Value = "Debe";
                ws.Cells[1, 4].Value = "Haber";
                ws.Cells[1, 5].Value = "Deudor";
                ws.Cells[1, 6].Value = "Acreedor";
                ws.Cells[1, 7].Value = "Activo";
                ws.Cells[1, 8].Value = "Pasivo";
                ws.Cells[1, 9].Value = "Resultado Debe";
                ws.Cells[1, 10].Value = "Resultado Haber";

                int row = 2;
                foreach (var item in Resultados)
                {
                    ws.Cells[row, 1].Value = item.Codigo;
                    ws.Cells[row, 2].Value = item.Cuenta;
                    ws.Cells[row, 3].Value = item.Debe;
                    ws.Cells[row, 4].Value = item.Haber;
                    ws.Cells[row, 5].Value = item.Deudor;
                    ws.Cells[row, 6].Value = item.Acreedor;
                    ws.Cells[row, 7].Value = item.Activo;
                    ws.Cells[row, 8].Value = item.Pasivo;
                    ws.Cells[row, 9].Value = item.ResultadoDebe;
                    ws.Cells[row, 10].Value = item.ResultadoHaber;
                    row++;
                }

                string tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"DetalleCuenta_{DateTime.Now:yyyyMMddHHmmss}.xlsx");

                package.SaveAs(new FileInfo(tempFile));
                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar a Excel: " + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Volver()
        {
            var main = (MainWindow)Application.Current.MainWindow;
        }
    }
}
