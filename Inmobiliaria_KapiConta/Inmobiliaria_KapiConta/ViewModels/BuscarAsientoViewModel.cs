using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Models.DTOs;
using Inmobiliaria_KapiConta.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class BuscarAsientoViewModel : INotifyPropertyChanged
    {
        // =========================
        // INFRAESTRUCTURA
        // =========================

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // DEPENDENCIAS EXTERNAS
        // =========================

        public Action<bool> CerrarVentana { get; set; }
        public Action<int> AsientoSeleccionado { get; set; }

        // =========================
        // COLECCIONES
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

        private ObservableCollection<AsientoBusquedaRegistroItem> _resultados;
        public ObservableCollection<AsientoBusquedaRegistroItem> Resultados
        {
            get => _resultados;
            set { _resultados = value; OnPropertyChanged(); }
        }

        // =========================
        // SELECCIONES
        // =========================

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

        private AsientoBusquedaRegistroItem _itemSeleccionado;
        public AsientoBusquedaRegistroItem ItemSeleccionado
        {
            get => _itemSeleccionado;
            set { _itemSeleccionado = value; OnPropertyChanged(); }
        }

        // =========================
        // TEXTOS
        // =========================

        private string _textoBuscar = "";
        public string TextoBuscar
        {
            get => _textoBuscar;
            set { _textoBuscar = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand BuscarCommand { get; }
        public ICommand SeleccionarCommand { get; }
        public ICommand CerrarCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public BuscarAsientoViewModel()
        {
            BuscarCommand = new RelayCommand(CargarBusqueda);
            SeleccionarCommand = new RelayCommand(SeleccionarActual);
            CerrarCommand = new RelayCommand(Cerrar);

            Resultados = new ObservableCollection<AsientoBusquedaRegistroItem>();

            CargarCombos();
            CargarBusqueda();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarCombos()
        {
            try
            {
                var mesService = new MesService();
                var meses = mesService.ObtenerMeses();

                // Agregar opción "Todos" al inicio
                var listaMeses = new ObservableCollection<Mes>();
                listaMeses.Add(new Mes { IdMes = 0, mes = "-- Todos --" });
                foreach (var m in meses)
                    listaMeses.Add(m);

                Meses = listaMeses;
                MesSeleccionado = Meses[0];

                var subDiarioService = new SubDiarioService();
                var subDiarios = subDiarioService.ObtenerSubDiarios();

                var listaSubDiarios = new ObservableCollection<SubDiario>();
                listaSubDiarios.Add(new SubDiario { IdSubDiario = 0, Diario = "-- Todos --" });
                foreach (var s in subDiarios)
                    listaSubDiarios.Add(s);

                SubDiarios = listaSubDiarios;
                SubDiarioSeleccionado = SubDiarios[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar filtros de búsqueda.\n\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =========================
        // BÚSQUEDA
        // =========================

        private void CargarBusqueda()
        {
            try
            {
                if (Session.CurrentEmpresa == null)
                    return;

                int? idMes = MesSeleccionado?.IdMes > 0 ? MesSeleccionado.IdMes : null;
                int? idSubDiario = SubDiarioSeleccionado?.IdSubDiario > 0 ? SubDiarioSeleccionado.IdSubDiario : null;

                string texto = TextoBuscar?.Trim() ?? "";

                var service = new AsientoService();
                var lista = service.BuscarAsientos(
                    Session.CurrentEmpresa.IdEmpresa,  // ✅ corrección aquí
                    idMes,
                    idSubDiario,
                    texto);

                Resultados = new ObservableCollection<AsientoBusquedaRegistroItem>(lista);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al buscar asientos.\n\n" + ex.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // =========================
        // SELECCIÓN Y CIERRE
        // =========================

        private void SeleccionarActual()
        {
            if (ItemSeleccionado == null)
            {
                MessageBox.Show("Selecciona un asiento.", "Buscar Asiento",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            AsientoSeleccionado?.Invoke(ItemSeleccionado.IdAsiento);
            CerrarVentana?.Invoke(true);
        }

        private void Cerrar()
        {
            CerrarVentana?.Invoke(false);
        }
    }
}