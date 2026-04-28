using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using OfficeOpenXml;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ListadoAsientoViewModel : INotifyPropertyChanged
    {
        private readonly AsientoService _service;
        private readonly MesService _mesService;
        private readonly SubDiarioService _subDiarioService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // COMBOS
        // =========================

        private ObservableCollection<Mes> _meses;
        public ObservableCollection<Mes> Meses
        {
            get => _meses;
            set { _meses = value; OnPropertyChanged(); }
        }

        private ObservableCollection<SubDiario> _subDiarios;
        public ObservableCollection<SubDiario> SubDiarios
        {
            get => _subDiarios;
            set { _subDiarios = value; OnPropertyChanged(); }
        }

        private Mes _mesSeleccionado;
        public Mes MesSeleccionado
        {
            get => _mesSeleccionado;
            set { _mesSeleccionado = value; OnPropertyChanged(); }
        }

        private SubDiario _subDiarioSeleccionado;
        public SubDiario SubDiarioSeleccionado
        {
            get => _subDiarioSeleccionado;
            set { _subDiarioSeleccionado = value; OnPropertyChanged(); }
        }

        // =========================
        // DATOS
        // =========================

        private DataView _asientos;
        public DataView Asientos
        {
            get => _asientos;
            set { _asientos = value; OnPropertyChanged(); }
        }

        private DataTable _dtAsientos;

        // =========================
        // COMMANDS
        // =========================

        public ICommand ProcesarCommand { get; }
        public ICommand ExportarExcelCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public ListadoAsientoViewModel()
        {
            _service = new AsientoService();
            _mesService = new MesService();
            _subDiarioService = new SubDiarioService();

            ExcelPackage.License.SetNonCommercialPersonal("Leonardo");

            ProcesarCommand = new RelayCommand(Procesar);
            ExportarExcelCommand = new RelayCommand(ExportarExcel);

            CargarCombos();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarCombos()
        {
            try
            {
                Meses = new ObservableCollection<Mes>(_mesService.ObtenerMeses());
                SubDiarios = new ObservableCollection<SubDiario>(_subDiarioService.ObtenerSubDiarios());

                // ✅ Seleccionar primeros por defecto
                if (Meses.Any()) MesSeleccionado = Meses[0];
                if (SubDiarios.Any()) SubDiarioSeleccionado = SubDiarios[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar combos: " + ex.Message);
            }
        }

        // =========================
        // PROCESAR
        // =========================

        private void Procesar()
        {
            try
            {
                if (MesSeleccionado == null)
                {
                    MessageBox.Show("Selecciona un mes.");
                    return;
                }

                if (SubDiarioSeleccionado == null)
                {
                    MessageBox.Show("Selecciona un subdiario.");
                    return;
                }

                _dtAsientos = _service.ObtenerAsientos(
                    Session.CurrentEmpresa.IdEmpresa,
                    MesSeleccionado.IdMes,
                    SubDiarioSeleccionado.Diario);

                Asientos = _dtAsientos.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        // =========================
        // EXPORTAR EXCEL
        // =========================

        private void ExportarExcel()
        {
            if (_dtAsientos == null || _dtAsientos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.");
                return;
            }

            try
            {
                string tempFile = Path.Combine(
                    Path.GetTempPath(),
                    $"Asientos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");

                using (var package = new ExcelPackage())
                {
                    var ws = package.Workbook.Worksheets.Add("Asientos");
                    ws.Cells["A1"].LoadFromDataTable(_dtAsientos, true);
                    ws.Cells[ws.Dimension.Address].AutoFitColumns();
                    package.SaveAs(new FileInfo(tempFile));
                }

                Process.Start(new ProcessStartInfo(tempFile) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al exportar: " + ex.Message);
            }
        }
    }
}
