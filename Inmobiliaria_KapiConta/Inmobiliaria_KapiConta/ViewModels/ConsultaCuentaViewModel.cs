// ViewModels/ConsultaCuentaViewModel.c
using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Models.DTOs;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Views.GestionListado;
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
    public class ConsultaCuentaViewModel : INotifyPropertyChanged
    {
        // ─── INotifyPropertyChanged ───────────────────────────────────
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // ─── Servicios ────────────────────────────────────────────────
        private readonly PlanCuentasService _planCuentasService;

        // ─── Comandos ─────────────────────────────────────────────────
        public ICommand ConsultarCommand { get; private set; }
        public ICommand ExportarCommand { get; private set; }
        public ICommand VolverCommand { get; private set; }
        public ICommand AbrirDetalleCommand { get; private set; }  

        // ─── Colecciones ──────────────────────────────────────────────
        public ObservableCollection<PlanCuenta> Cuentas { get; } = new();
        public ObservableCollection<ConsultaCuentaDto> Resultados { get; } = new();

        // ─── Propiedades ──────────────────────────────────────────────

        private PlanCuenta _cuentaSeleccionada;
        public PlanCuenta CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set
            {
                _cuentaSeleccionada = value;
                OnPropertyChanged();

                // Al seleccionar del ListBox, sincroniza el TextBox y consulta
                if (value != null)
                {
                    CodigoCuenta = value.Codigo;
                    Consultar();
                }
            }
        }

        private string _codigoCuenta = string.Empty;
        public string CodigoCuenta
        {
            get => _codigoCuenta;
            set { _codigoCuenta = value; OnPropertyChanged(); }
        }

        private string _saldoTotal = "SALDO CUENTA: 0.00";
        public string SaldoTotal
        {
            get => _saldoTotal;
            set { _saldoTotal = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Fila seleccionada en el DataGrid.
        /// Bindeada a SelectedItem — lista para cuando implementes AbrirDetalle.
        /// </summary>
        private ConsultaCuentaDto _filaSeleccionada;
        public ConsultaCuentaDto FilaSeleccionada
        {
            get => _filaSeleccionada;
            set { _filaSeleccionada = value; OnPropertyChanged(); }
        }

        // ─── Constructor ──────────────────────────────────────────────
        public ConsultaCuentaViewModel()
        {
            _planCuentasService = new PlanCuentasService(Session.CurrentEmpresa.IdEmpresa);

            Inicializar();
        }

        private void Inicializar()
        {
            ConsultarCommand = new RelayCommand(Consultar);
            ExportarCommand = new RelayCommand(Exportar, () => Resultados.Any());
            VolverCommand = new RelayCommand(Volver);
            AbrirDetalleCommand = new RelayCommand(AbrirDetalle, () => FilaSeleccionada != null);

            CargarCuentas();
        }

        // ─── Métodos privados ─────────────────────────────────────────

        private void CargarCuentas()
        {
            try
            {
                var lista = _planCuentasService.ObtenerPlanCuentas();
                Cuentas.Clear();
                foreach (var c in lista)
                    Cuentas.Add(c);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar cuentas: " + ex.Message);
            }
        }

        private void Consultar()
        {
            if (string.IsNullOrWhiteSpace(CodigoCuenta)) return;

            try
            {
                Resultados.Clear();

                using var conn = DbConnectionFactory.Create();
                conn.Open();

                const string query = @"
                    SELECT 
                        m.nombre AS mes,
                        COALESCE(SUM(ad.debe),  0) AS debe,
                        COALESCE(SUM(ad.haber), 0) AS haber,
                        COALESCE(SUM(ad.debe - ad.haber), 0) AS periodo,
                        SUM(COALESCE(SUM(ad.debe - ad.haber), 0))
                            OVER (ORDER BY m.id_mes) AS acumulado
                    FROM mes m
                    LEFT JOIN asiento a 
                        ON a.id_mes     = m.id_mes
                       AND a.id_empresa = @empresa
                    LEFT JOIN asiento_detalle ad 
                        ON ad.id_asiento = a.id_asiento
                       AND ad.id_plan_cuenta = (
                            SELECT id_plan_cuenta
                            FROM plan_cuenta
                            WHERE codigo     = @cuenta
                              AND id_empresa = @empresa
                            LIMIT 1
                       )
                    GROUP BY m.id_mes, m.nombre
                    ORDER BY m.id_mes;";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cuenta", CodigoCuenta);
                cmd.Parameters.AddWithValue("@empresa", Session.CurrentEmpresa.IdEmpresa);

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Resultados.Add(new ConsultaCuentaDto
                    {
                        Mes = dr["mes"].ToString(),
                        Debe = (decimal)dr["debe"],
                        Haber = (decimal)dr["haber"],
                        Periodo = (decimal)dr["periodo"],
                        Acumulado = (decimal)dr["acumulado"]
                    });
                }

                decimal total = Resultados.LastOrDefault()?.Acumulado ?? 0m;
                SaldoTotal = $"SALDO CUENTA: {total:N2}";

                ((RelayCommand)ExportarCommand).RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar: " + ex.Message);
            }
        }

        private void Exportar()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Leonardo");

            try
            {
                using var package = new ExcelPackage();
                var ws = package.Workbook.Worksheets.Add("Consulta");

                ws.Cells[1, 1].Value = "Mes";
                ws.Cells[1, 2].Value = "Debe";
                ws.Cells[1, 3].Value = "Haber";
                ws.Cells[1, 4].Value = "Periodo";
                ws.Cells[1, 5].Value = "Acumulado";

                int row = 2;
                foreach (var item in Resultados)
                {
                    ws.Cells[row, 1].Value = item.Mes;
                    ws.Cells[row, 2].Value = item.Debe;
                    ws.Cells[row, 3].Value = item.Haber;
                    ws.Cells[row, 4].Value = item.Periodo;
                    ws.Cells[row, 5].Value = item.Acumulado;
                    row++;
                }

                string tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"Consulta_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                File.WriteAllBytes(tempFile, package.GetAsByteArray());
                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al exportar: {ex.Message}");
            }
        }

        private void Volver()
        {
            var main = (MainWindow)Application.Current.MainWindow;
        }

        // ─── Preparado para doble click ───────────────────────────────
        private void AbrirDetalle()
        {
            if (FilaSeleccionada == null) return;

            string mesCodigo = ObtenerCodigoMes(FilaSeleccionada.Mes);

            var vm = new DetalleCuentaViewModel(CodigoCuenta, mesCodigo);

            var ventana = new DetalleCuentaWindow
            {
                DataContext = vm,
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)
            };

            // El VM necesita saber cómo cerrarse sin conocer la Window
            vm.Cerrar = () => ventana.Close();

            ventana.ShowDialog();
        }

        private static string ObtenerCodigoMes(string nombre) => nombre.ToLower() switch
        {
            "apertura" => "00",
            "enero" => "01",
            "febrero" => "02",
            "marzo" => "03",
            "abril" => "04",
            "mayo" => "05",
            "junio" => "06",
            "julio" => "07",
            "agosto" => "08",
            "septiembre" => "09",
            "octubre" => "10",
            "noviembre" => "11",
            "diciembre" => "12",
            "cierre" => "13",
            _ => "00"
        };
    }
}