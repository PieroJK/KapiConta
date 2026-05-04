using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Views.GestionAsiento;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class AgregarDetalleAsientoViewModel : INotifyPropertyChanged
    {
        private readonly PlanCuentasService _planCuentasService;
        private readonly TerceroService _terceroService;
        private readonly TipoOperacionAsientoService _tipoOperacionService;
        private readonly TipoFacturacionService _tipoFacturacionService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // PARÁMETROS
        // =========================

        public string MonedaCabecera { get; }
        public Action Cerrar { get; set; }

        // =========================
        // COMBOS
        // =========================

        public ObservableCollection<string> OpcionesDebeHaber { get; } = new()
        {
            "DEBE", "HABER"
        };

        private ObservableCollection<TipoOperacionAsiento> _tiposOperacion;
        public ObservableCollection<TipoOperacionAsiento> TiposOperacion
        {
            get => _tiposOperacion;
            set { _tiposOperacion = value; OnPropertyChanged(); }
        }

        private ObservableCollection<TipoFacturacion> _tiposDocumento;
        public ObservableCollection<TipoFacturacion> TiposDocumento
        {
            get => _tiposDocumento;
            set { _tiposDocumento = value; OnPropertyChanged(); }
        }

        // =========================
        // SELECCIONES COMBOS
        // =========================

        private string _debeHaberSeleccionado = "DEBE";
        public string DebeHaberSeleccionado
        {
            get => _debeHaberSeleccionado;
            set { _debeHaberSeleccionado = value; OnPropertyChanged(); }
        }

        private TipoOperacionAsiento _tipoOperacionSeleccionado;
        public TipoOperacionAsiento TipoOperacionSeleccionado
        {
            get => _tipoOperacionSeleccionado;
            set { _tipoOperacionSeleccionado = value; OnPropertyChanged(); }
        }

        private TipoFacturacion _tipoDocumentoSeleccionado;
        public TipoFacturacion TipoDocumentoSeleccionado
        {
            get => _tipoDocumentoSeleccionado;
            set
            {
                _tipoDocumentoSeleccionado = value;
                OnPropertyChanged();
                TipoDocumentoNombre = value?.Nombre ?? "";
            }
        }

        // =========================
        // CUENTA
        // =========================

        private string _cuentaCodigo;
        public string CuentaCodigo
        {
            get => _cuentaCodigo;
            set { _cuentaCodigo = value; OnPropertyChanged(); }
        }

        private string _cuentaNombre;
        public string CuentaNombre
        {
            get => _cuentaNombre;
            set { _cuentaNombre = value; OnPropertyChanged(); }
        }

        private string _saldoCuenta = "0.00";
        public string SaldoCuenta
        {
            get => _saldoCuenta;
            set { _saldoCuenta = value; OnPropertyChanged(); }
        }

        private bool _tieneAutomatizacion;
        public bool TieneAutomatizacion
        {
            get => _tieneAutomatizacion;
            set { _tieneAutomatizacion = value; OnPropertyChanged(); }
        }

        private bool _usarAutomatizacion;
        public bool UsarAutomatizacion
        {
            get => _usarAutomatizacion;
            set { _usarAutomatizacion = value; OnPropertyChanged(); }
        }

        private PlanCuenta _cuentaSeleccionada;

        // =========================
        // TERCERO
        // =========================

        private string _ruc;
        public string Ruc
        {
            get => _ruc;
            set { _ruc = value; OnPropertyChanged(); }
        }

        private string _razonSocial;
        public string RazonSocial
        {
            get => _razonSocial;
            set { _razonSocial = value; OnPropertyChanged(); }
        }

        private Tercero _terceroSeleccionado;

        // =========================
        // OTROS CAMPOS
        // =========================

        private string _importe = "0.00";
        public string Importe
        {
            get => _importe;
            set { _importe = value; OnPropertyChanged(); }
        }

        private string _tipoDocumentoNombre;
        public string TipoDocumentoNombre
        {
            get => _tipoDocumentoNombre;
            set { _tipoDocumentoNombre = value; OnPropertyChanged(); }
        }

        private string _documento;
        public string Documento
        {
            get => _documento;
            set { _documento = value; OnPropertyChanged(); }
        }

        private string _glosa;
        public string Glosa
        {
            get => _glosa;
            set { _glosa = value; OnPropertyChanged(); }
        }

        private string _asientoReferencia;
        public string AsientoReferencia
        {
            get => _asientoReferencia;
            set { _asientoReferencia = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand AbrirSelectorCuentaCommand { get; set; }
        public ICommand CuentaCodigoLostFocusCommand { get; set; }
        public ICommand AbrirSelectorTerceroCommand { get; set; }
        public ICommand RucLostFocusCommand { get; set; }
        public ICommand AbrirSelectorAsientoReferenciaCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand CancelarCommand { get; set; }

        // =========================
        // LISTAS INTERNAS
        // =========================

        private List<PlanCuenta> _listaCuentas = new();
        private List<Tercero> _listaTerceros = new();

        // =========================
        // CONSTRUCTOR
        // =========================

        public AgregarDetalleAsientoViewModel(string monedaCabecera = "PEN")
        {
            MonedaCabecera = monedaCabecera;

            _planCuentasService = new PlanCuentasService(Session.CurrentEmpresa.IdEmpresa);
            _terceroService = new TerceroService();
            _tipoOperacionService = new TipoOperacionAsientoService();
            _tipoFacturacionService = new TipoFacturacionService();

            Inicializar(); // 👈 solo comandos y carga
        }

        private void Inicializar()
        {
            AbrirSelectorCuentaCommand = new RelayCommand(AbrirSelectorCuenta);
            CuentaCodigoLostFocusCommand = new RelayCommand(BuscarCuentaPorCodigo);
            AbrirSelectorTerceroCommand = new RelayCommand(AbrirSelectorTercero);
            RucLostFocusCommand = new RelayCommand(BuscarTerceroPorRuc);
            AbrirSelectorAsientoReferenciaCommand = new RelayCommand(AbrirSelectorAsientoReferencia);
            AgregarCommand = new RelayCommand(Agregar);
            CancelarCommand = new RelayCommand(Cancelar);

            CargarDatos();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarDatos()
        {
            try
            {
                _listaCuentas = _planCuentasService.ObtenerPlanCuentas();
                _listaTerceros = _terceroService.Listar();

                TiposOperacion = new ObservableCollection<TipoOperacionAsiento>(
                    _tipoOperacionService.ObtenerTiposOperacion());

                TiposDocumento = new ObservableCollection<TipoFacturacion>(
                    _tipoFacturacionService.ObtenerTiposFacturacion());

                if (TiposOperacion.Any())
                    TipoOperacionSeleccionado = TiposOperacion[0];

                if (TiposDocumento.Any())
                    TipoDocumentoSeleccionado = TiposDocumento[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
        }

        // =========================
        // CUENTA
        // =========================

        private void AbrirSelectorCuenta()
        {
            var vm = new SelectorCuentaViewModel();

            var win = new SelectorCuentaWindow
            {
                DataContext = vm,
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)
            };

            vm.OnCuentaSeleccionada = item =>
            {
                AplicarCuenta(item.CodigoCuenta, item.NombreCuenta);
                win.Close(); 
            };

            win.ShowDialog();
        }

        private void BuscarCuentaPorCodigo()
        {
            var cuenta = _listaCuentas.FirstOrDefault(x =>
                x.Codigo.Equals(CuentaCodigo?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (cuenta == null) { LimpiarCuenta(); return; }

            _cuentaSeleccionada = cuenta;
            AplicarCuenta(cuenta.Codigo, cuenta.Descripcion);
        }

        private void AplicarCuenta(string codigo, string nombre)
        {
            var cuenta = _listaCuentas.FirstOrDefault(x =>
                x.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase));

            _cuentaSeleccionada = cuenta;
            CuentaCodigo = codigo;
            CuentaNombre = nombre;
            TieneAutomatizacion = cuenta?.TieneAutomatizacion ?? false;
            UsarAutomatizacion = TieneAutomatizacion;
            SaldoCuenta = "0.00"; // 🔜 calcular saldo real si se necesita
        }

        private void LimpiarCuenta()
        {
            _cuentaSeleccionada = null;
            CuentaNombre = "";
            SaldoCuenta = "0.00";
            TieneAutomatizacion = false;
            UsarAutomatizacion = false;
        }

        // =========================
        // TERCERO
        // =========================

        private void AbrirSelectorTercero()
        {
            var vm = new SelectorTerceroViewModel();

            var win = new SelectorTerceroWindow
            {
                DataContext = vm,
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)
            };

            vm.OnTerceroSeleccionado = tercero =>
            {
                AplicarTercero(tercero);
                win.Close(); // 🔥 equivalente a DialogResult + Close
            };

            win.ShowDialog();
        }

        private void BuscarTerceroPorRuc()
        {
            var tercero = _listaTerceros.FirstOrDefault(x =>
                x.Documento.Equals(Ruc?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (tercero == null) { LimpiarTercero(); return; }
            AplicarTercero(tercero);
        }

        private void AplicarTercero(Tercero tercero)
        {
            _terceroSeleccionado = tercero;
            Ruc = tercero.Documento;
            RazonSocial = tercero.RazonSocial;
        }

        private void LimpiarTercero()
        {
            _terceroSeleccionado = null;
            RazonSocial = "";
        }

        // =========================
        // ASIENTO REFERENCIA
        // =========================

        private void AbrirSelectorAsientoReferencia()
        {
            if (TipoDocumentoSeleccionado?.Cod != "07" &&
                TipoDocumentoSeleccionado?.Cod != "08")
            {
                MessageBox.Show(
                    "El asiento de referencia solo es para Nota de Crédito o Nota de Débito.",
                    "Asientos", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 🔜 Abrir SelectorAsientoReferenciaWindow cuando esté disponible
            MessageBox.Show("Selector de asiento de referencia pendiente.");
        }

        // =========================
        // AGREGAR (pendiente)
        // =========================

        private void Agregar()
        {
            // 🔜 Se implementará después
        }

        private void Cancelar()
        {
            Cerrar?.Invoke();
        }
    }
}
