using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class PlanCuentasViewModel : INotifyPropertyChanged 
    {
        private readonly PlanCuentasService _service;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // PROPIEDADES PRINCIPALES
        // =========================

        public ObservableCollection<PlanCuentaItem> PlanCuentas { get; set; } = new();

        private PlanCuentaItem _cuentaSeleccionada;
        public PlanCuentaItem CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set
            {
                _cuentaSeleccionada = value;
                OnPropertyChanged();
                CargarDetalleSeleccionado();
            }
        }

        // =========================
        // FORMULARIO
        // =========================

        private string _codigo;
        public string Codigo
        {
            get => _codigo;
            set
            {
                _codigo = value;
                Nivel = CalcularNivelDesdeCodigo(value);
                OnPropertyChanged();
            }
        }

        private string _descripcion;
        public string Descripcion
        {
            get => _descripcion;
            set { _descripcion = value; OnPropertyChanged(); }
        }

        private int _nivel;
        public int Nivel
        {
            get => _nivel;
            set { _nivel = value; OnPropertyChanged(); }
        }

        private string _codigoPadre;
        public string CodigoPadre
        {
            get => _codigoPadre;
            set { _codigoPadre = value; OnPropertyChanged(); }
        }

        private int? _idElemento;
        public int? IdElemento
        {
            get => _idElemento;
            set { _idElemento = value; OnPropertyChanged(); }
        }

        private int? _idBalance;
        public int? IdBalance
        {
            get => _idBalance;
            set { _idBalance = value; OnPropertyChanged(); }
        }

        private bool _analisis;
        public bool Analisis
        {
            get => _analisis;
            set { _analisis = value; OnPropertyChanged(); }
        }

        private bool _tieneAutomatizacion;
        public bool TieneAutomatizacion
        {
            get => _tieneAutomatizacion;
            set { _tieneAutomatizacion = value; OnPropertyChanged(); }
        }

        // =========================
        // COMBOS
        // =========================

        public ObservableCollection<ComboItem> Elementos { get; set; } = new();
        public ObservableCollection<ComboItem> Balances { get; set; } = new();
        public ObservableCollection<CuentaPadreItem> CuentasPadre { get; set; } = new();

        // =========================
        // AUTOMATIZACIÓN
        // =========================

        public ObservableCollection<AutomatizacionDetalleView> Automatizacion { get; set; } = new();

        // =========================
        // COMMANDS
        // =========================

        public ICommand AgregarCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand RefrescarCommand { get; }
        public ICommand AsientoMasCommand { get; }
        public ICommand AsientoMenosCommand { get; }

        public ICommand GuardarAutomatizacionCommand{ get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public PlanCuentasViewModel(int empresaId)
        {
            _service = new PlanCuentasService(empresaId);

            AgregarCommand = new RelayCommand(Agregar);
            ModificarCommand = new RelayCommand(Modificar);
            EliminarCommand = new RelayCommand(Eliminar);

            AsientoMasCommand = new RelayCommand(AsientoMas);
            AsientoMenosCommand = new RelayCommand(AsientoMenos);
            GuardarAutomatizacionCommand = new RelayCommand(guardarautomatizacion);

            CargarInicial();
        }


        // =========================
        // CARGA INICIAL
        // =========================

        public void CargarInicial()
        {
            PlanCuentas = new ObservableCollection<PlanCuentaItem>(_service.ObtenerPlanCuentas());
            Elementos = new ObservableCollection<ComboItem>(_service.ObtenerElementos());
            Balances = new ObservableCollection<ComboItem>(_service.ObtenerBalances());

            OnPropertyChanged(nameof(PlanCuentas));
            OnPropertyChanged(nameof(Elementos));
            OnPropertyChanged(nameof(Balances));
        }

        // =========================
        // SELECCIÓN
        // =========================

        private void CargarDetalleSeleccionado()
        {
            if (CuentaSeleccionada == null) return;

            Codigo = CuentaSeleccionada.Codigo;
            Descripcion = CuentaSeleccionada.Descripcion;
            
            CodigoPadre = CuentaSeleccionada.CodigoPadre;
            IdElemento = CuentaSeleccionada.IdElemento;
            IdBalance = CuentaSeleccionada.IdBalance;
            Analisis = CuentaSeleccionada.Analisis;
            TieneAutomatizacion = CuentaSeleccionada.TieneAutomatizacion;
        }

        // =========================
        // CRUD
        // =========================

        private void Agregar()
        {
            if (string.IsNullOrWhiteSpace(Codigo) || string.IsNullOrWhiteSpace(Descripcion))
                return;
            _service.InsertarCuenta(new PlanCuentaItem
            {
                Codigo = Codigo,
                Descripcion = Descripcion,
                Nivel = Nivel,
                CodigoPadre = CodigoPadre,
                IdElemento = IdElemento ?? 0,
                IdBalance = IdBalance,
                Analisis = Analisis
            });
            

            Refrescar();
        }

        private void Modificar()
        {
            if (CuentaSeleccionada == null) return;
            _service.ModificarCuenta(new PlanCuentaItem
            {
                IdPlanCuenta = CuentaSeleccionada.IdPlanCuenta,
                Codigo = Codigo,
                Descripcion = Descripcion,
                Nivel = Nivel,
                CodigoPadre = CodigoPadre,
                IdElemento = IdElemento ?? 0,
                IdBalance = IdBalance,
                Analisis = Analisis
            });

            Refrescar();
        }

        private void Eliminar()
        {
            if (CuentaSeleccionada == null) return;
            Refrescar();

            _service.EliminarCuenta(CuentaSeleccionada.IdPlanCuenta);
            Refrescar();
        }

        private void Refrescar()
        {
            CargarInicial();
            Limpiar();
        }

        // =========================
        // UTILS
        // =========================

        private void Limpiar()
        {
            CuentaSeleccionada = null;
            Codigo = "";
            Descripcion = "";
            Nivel = 0;
            CodigoPadre = null;
            IdElemento = null;
            IdBalance = null;
            Analisis = false;
            TieneAutomatizacion = false;
            Automatizacion.Clear();
        }

        private void AsientoMas()
        {
            if (CuentaSeleccionada == null)
            {
                System.Windows.MessageBox.Show("Selecciona una cuenta primero.");
                return;
            }

            System.Windows.MessageBox.Show("Aquí luego abriremos la ventana para agregar automatización.");
        }

        private void AsientoMenos()
        {
            if (CuentaSeleccionada == null)
            {
                System.Windows.MessageBox.Show("Selecciona una cuenta primero.");
                return;
            }

            System.Windows.MessageBox.Show("Aquí luego quitaremos el detalle de automatización seleccionado.");
        }

        private void guardarautomatizacion()
        {
            if (CuentaSeleccionada == null)
            {
                System.Windows.MessageBox.Show("Selecciona una cuenta primero.");
                return;
            }

            System.Windows.MessageBox.Show("Aquí luego guardaremos la automatizacion.");
        }
        private int CalcularNivelDesdeCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return 0;

            return codigo.Trim().Length;
        }
    }
}
