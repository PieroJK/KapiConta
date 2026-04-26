using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Models.DTOs;
using Inmobiliaria_KapiConta.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ImportarAsientoViewModel : INotifyPropertyChanged
    {
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
        // ARCHIVO
        // =========================

        private string _rutaArchivo;
        public string RutaArchivo
        {
            get => _rutaArchivo;
            set { _rutaArchivo = value; OnPropertyChanged(); }
        }

        // =========================
        // MODO IMPORTACIÓN
        // =========================

        private bool _modoInicio = true;
        public bool ModoInicio
        {
            get => _modoInicio;
            set { _modoInicio = value; OnPropertyChanged(); }
        }

        private bool _modoContinuar;
        public bool ModoContinuar
        {
            get => _modoContinuar;
            set { _modoContinuar = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand SeleccionarArchivoCommand { get; }
        public ICommand ImportarCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public ImportarAsientoViewModel()
        {
            _mesService = new MesService();
            _subDiarioService = new SubDiarioService();

            SeleccionarArchivoCommand = new RelayCommand(SeleccionarArchivo);
            ImportarCommand = new RelayCommand(Importar);

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        // =========================
        // ARCHIVO
        // =========================

        private void SeleccionarArchivo()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Seleccionar archivo Excel",
                Filter = "Archivos Excel|*.xlsx;*.xls"
            };

            if (dialog.ShowDialog() == true)
                RutaArchivo = dialog.FileName;
        }

        // =========================
        // IMPORTAR (pendiente)
        // =========================

        private void Importar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RutaArchivo))
                {
                    MessageBox.Show("Selecciona un archivo Excel.");
                    return;
                }

                if (SubDiarioSeleccionado == null)
                {
                    MessageBox.Show("Selecciona el subdiario.");
                    return;
                }

                if (MesSeleccionado == null)
                {
                    MessageBox.Show("Selecciona el mes.");
                    return;
                }

                var service = new ImportarAsientoService();

                // ✅ Compras: diario "08" | Ventas: diario "14"
                bool esCompra = SubDiarioSeleccionado.Diario == "C";

                ImportacionResult result = esCompra
                    ? service.ImportarCompras(RutaArchivo, MesSeleccionado.IdMes, ModoInicio)
                    : service.ImportarVentas(RutaArchivo, MesSeleccionado.IdMes, ModoInicio);

                if (result.Exitoso)
                {
                    MessageBox.Show($"Importación correcta. {result.AsientosImportados} asientos importados.");
                }
                else
                {
                    string errores = string.Join("\n", result.Errores);
                    MessageBox.Show($"Errores encontrados:\n\n{errores}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado: " + ex.Message);
            }
        }
    }
}
