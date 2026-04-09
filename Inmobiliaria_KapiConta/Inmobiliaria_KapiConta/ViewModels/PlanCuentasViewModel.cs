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
        private readonly ElementoService _elementoService;
        private readonly BalanceService _balanceService;

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
                (ModificarCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EliminarCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        private Elemento _elementoSeleccionado;
        public Elemento ElementoSeleccionado
        {
            get => _elementoSeleccionado;
            set
            {
                _elementoSeleccionado = value;
                OnPropertyChanged();
            }
        }

        private Balance _balanceSeleccionado;
        public Balance BalanceSeleccionado
        {
            get => _balanceSeleccionado;
            set
            {
                _balanceSeleccionado = value;
                OnPropertyChanged();
            }
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

        public ObservableCollection<Elemento> Elementos { get; set; } = new();
        public ObservableCollection<Balance> Balances { get; set; } = new();
        public ObservableCollection<CuentaPadreItem> CuentasPadre { get; set; } = new();

        // =========================
        // AUTOMATIZACIÓN
        // =========================

        public ObservableCollection<AutomatizacionDetalleItem> Automatizacion { get; set; } = new();

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
            _elementoService = new ElementoService();
            _balanceService = new BalanceService();

            AgregarCommand = new RelayCommand(Agregar);
            ModificarCommand = new RelayCommand(Modificar, () => CuentaSeleccionada != null);
            EliminarCommand = new RelayCommand(Eliminar, () => CuentaSeleccionada != null);

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
            Elementos = new ObservableCollection<Elemento>(_elementoService.ObtenerElementos());
            Balances = new ObservableCollection<Balance>(_balanceService.ObtenerBalances());
            CuentasPadre = new ObservableCollection<CuentaPadreItem>(_service.ObtenerCuentasPadre());

            OnPropertyChanged(nameof(PlanCuentas));
            OnPropertyChanged(nameof(Elementos));
            OnPropertyChanged(nameof(Balances));
            OnPropertyChanged(nameof(CuentasPadre));
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
            ElementoSeleccionado = Elementos.FirstOrDefault(x => x.IdElemento == CuentaSeleccionada.IdElemento);
            BalanceSeleccionado = Balances.FirstOrDefault(x => x.IdBalance == CuentaSeleccionada.IdBalance);
            Analisis = CuentaSeleccionada.Analisis;

            // 🔹 AQUÍ ESTÁ LO IMPORTANTE
            var result = _service.ObtenerAutomatizacion(CuentaSeleccionada.IdPlanCuenta);

            Automatizacion = new ObservableCollection<AutomatizacionDetalleItem>(result.lista);
            TieneAutomatizacion = result.estado;

            OnPropertyChanged(nameof(Automatizacion));
            OnPropertyChanged(nameof(TieneAutomatizacion));
        }

        // =========================
        // CRUD
        // =========================

        private void Agregar()
        {
            try
            {
                // ❗ VALIDACIONES (equivalentes a tu código anterior)

                if (string.IsNullOrWhiteSpace(Codigo))
                {
                    System.Windows.MessageBox.Show("Ingresa el código.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Descripcion))
                {
                    System.Windows.MessageBox.Show("Ingresa la descripción.");
                    return;
                }

                if (ElementoSeleccionado == null)
                {
                    System.Windows.MessageBox.Show("Selecciona el elemento.");
                    return;
                }

                // ⚠ Antes usabas ComboBox (Sí/No), ahora es bool
                // así que no necesitas validar SelectedIndex

                // ❗ VALIDAR SI YA EXISTE
                if (_service.ExisteCodigo(Codigo))
                {
                    System.Windows.MessageBox.Show("Ya existe una cuenta con ese código.");
                    return;
                }

                //  INSERTAR
                _service.InsertarCuenta(new PlanCuentaItem
                {
                    Codigo = Codigo,
                    Descripcion = Descripcion,
                    Nivel = Nivel,
                    CodigoPadre = CodigoPadre,
                    IdElemento = ElementoSeleccionado.IdElemento,
                    IdBalance = BalanceSeleccionado?.IdBalance,
                    Analisis = Analisis
                });

                // RECARGAR
                Refrescar();

                System.Windows.MessageBox.Show("Cuenta agregada correctamente.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void Modificar()
        {
            try
            {
                if (CuentaSeleccionada == null)
                {
                    System.Windows.MessageBox.Show("Selecciona una cuenta.");
                    return;
                }

                if (CuentaSeleccionada.EsBase)
                {
                    System.Windows.MessageBox.Show("Las cuentas base no se pueden modificar.");
                    return;
                }

                var respuesta = System.Windows.MessageBox.Show(
                    "¿Estás seguro de modificar esta cuenta?",
                    "Confirmar modificación",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Question);

                if (respuesta != System.Windows.MessageBoxResult.Yes)
                    return;

                // VALIDACIONES
                if (string.IsNullOrWhiteSpace(Codigo))
                {
                    System.Windows.MessageBox.Show("Ingresa el código.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Descripcion))
                {
                    System.Windows.MessageBox.Show("Ingresa la descripción.");
                    return;
                }

                if (ElementoSeleccionado == null)
                {
                    System.Windows.MessageBox.Show("Selecciona el elemento.");
                    return;
                }

                // VALIDAR DUPLICADO (IMPORTANTE)
                if (_service.ExisteCodigo(Codigo, CuentaSeleccionada.IdPlanCuenta))
                {
                    System.Windows.MessageBox.Show("Ya existe una cuenta con ese código.");
                    return;
                }

                // MODIFICAR
                _service.ModificarCuenta(new PlanCuentaItem
                {
                    IdPlanCuenta = CuentaSeleccionada.IdPlanCuenta,
                    Codigo = Codigo,
                    Descripcion = Descripcion,
                    Nivel = Nivel,
                    CodigoPadre = CodigoPadre,
                    IdElemento = ElementoSeleccionado.IdElemento,
                    IdBalance = BalanceSeleccionado?.IdBalance,
                    Analisis = Analisis
                });

                Refrescar();

                System.Windows.MessageBox.Show("Cuenta modificada correctamente.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al modificar: " + ex.Message);
            }
        }

        private void Eliminar()
        {
            try
            {
                if (CuentaSeleccionada == null)
                {
                    System.Windows.MessageBox.Show("Selecciona una cuenta.");
                    return;
                }

                if (CuentaSeleccionada.EsBase)
                {
                    System.Windows.MessageBox.Show("Las cuentas base no se pueden eliminar.");
                    return;
                }

                var respuesta = System.Windows.MessageBox.Show(
                    "¿Estás seguro de eliminar esta cuenta?",
                    "Confirmar eliminación",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (respuesta != System.Windows.MessageBoxResult.Yes)
                    return;

                _service.EliminarCuenta(CuentaSeleccionada.IdPlanCuenta);

                Refrescar();

                System.Windows.MessageBox.Show("Cuenta eliminada correctamente.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error al eliminar: " + ex.Message);
            }
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
            ElementoSeleccionado = null;
            BalanceSeleccionado = null;
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
            try
            {
                if (CuentaSeleccionada == null)
                {
                    System.Windows.MessageBox.Show("Selecciona una cuenta.");
                    return;
                }

                // 🔴 SI ESTA DESACTIVADO → NO VALIDAR DETALLE
                if (!TieneAutomatizacion)
                {
                    _service.GuardarAutomatizacion(
                        CuentaSeleccionada.IdPlanCuenta,
                        new List<AutomatizacionDetalleItem>(),
                        false
                    );

                    System.Windows.MessageBox.Show("Automatización desactivada.");
                    return;
                }

                // 🔹 VALIDACIONES SOLO SI ESTA ACTIVO
                if (Automatizacion.Count == 0)
                {
                    System.Windows.MessageBox.Show("Agrega al menos una cuenta.");
                    return;
                }

                decimal totalDebe = Automatizacion
                    .Where(x => x.TipoMovimiento == "D")
                    .Sum(x => x.Porcentaje);

                decimal totalHaber = Automatizacion
                    .Where(x => x.TipoMovimiento == "H")
                    .Sum(x => x.Porcentaje);

                if (totalDebe <= 0)
                {
                    System.Windows.MessageBox.Show("Debe existir al menos una línea en Debe.");
                    return;
                }

                if (totalHaber <= 0)
                {
                    System.Windows.MessageBox.Show("Debe existir al menos una línea en Haber.");
                    return;
                }

                if (totalDebe != 100m)
                {
                    System.Windows.MessageBox.Show($"Debe = 100. Actual: {totalDebe}");
                    return;
                }

                if (totalHaber != 100m)
                {
                    System.Windows.MessageBox.Show($"Haber = 100. Actual: {totalHaber}");
                    return;
                }

                _service.GuardarAutomatizacion(
                    CuentaSeleccionada.IdPlanCuenta,
                    Automatizacion.ToList(),
                    true
                );

                System.Windows.MessageBox.Show("Automatización guardada correctamente.");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error: " + ex.Message);
            }
        }
        private int CalcularNivelDesdeCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return 0;

            return codigo.Trim().Length;
        }
    }
}
