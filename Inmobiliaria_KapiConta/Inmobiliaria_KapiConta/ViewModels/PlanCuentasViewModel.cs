using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class PlanCuentasViewModel : INotifyPropertyChanged 
    {
        private readonly PlanCuentasService _service;
        private readonly ElementoService _elementoService;
        private readonly BalanceService _balanceService;
        private readonly AutomatizacionService _automatizacionService;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // PROPIEDADES PRINCIPALES
        // =========================

        private ObservableCollection<PlanCuenta> _planCuentas;
        public ObservableCollection<PlanCuenta> PlanCuentas
        {
            get => _planCuentas;
            set
            {
                _planCuentas = value;
                OnPropertyChanged();
            }
        }

        private PlanCuenta _cuentaSeleccionada;
        public PlanCuenta CuentaSeleccionada
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
        // BÚSQUEDA
        // =========================

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                // ✅ Aplicar filtro en tiempo real
                CollectionViewSource.GetDefaultView(PlanCuentas)?.Refresh();
            }
        }

        // =========================
        // ELIMINAR ATUMATIZACIÓN
        // =========================

        private AutomatizacionDetalleItem _automatizacionSeleccionada;
        public AutomatizacionDetalleItem AutomatizacionSeleccionada
        {
            get => _automatizacionSeleccionada;
            set
            {
                _automatizacionSeleccionada = value;
                OnPropertyChanged();
            }
        }
        // ✅ Llamar esto después de asignar PlanCuentas en CargarDatos()
        private void AplicarFiltro()
        {
            var view = CollectionViewSource.GetDefaultView(PlanCuentas);
            view.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(TextoBusqueda)) return true;
                var cuenta = item as PlanCuenta;
                return cuenta.Codigo.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                    || cuenta.Descripcion.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase);
            };
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
                if (_codigo == value) return;

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

        private ObservableCollection<PlanCuenta> _cuentasPadre;
        public ObservableCollection<PlanCuenta> CuentasPadre
        {
            get => _cuentasPadre;
            set
            {
                _cuentasPadre = value;
                OnPropertyChanged();
            }
        }

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
            _automatizacionService = new AutomatizacionService();

            AgregarCommand = new RelayCommand(Agregar);
            ModificarCommand = new RelayCommand(Modificar, () => CuentaSeleccionada != null && !CuentaSeleccionada.EsBase);
            EliminarCommand = new RelayCommand(Eliminar, () => CuentaSeleccionada != null && !CuentaSeleccionada.EsBase);

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
            CargarDatos();
            CargarCombos();
        }

        // =========================
        // SELECCIÓN
        // =========================

        private void CargarDatos()
        {
            var lista = _service.ObtenerPlanCuentas();
            PlanCuentas = new ObservableCollection<PlanCuenta>(lista);
            CuentasPadre = new ObservableCollection<PlanCuenta>(lista);
            AplicarFiltro(); // ✅ Siempre aplicar tras recargar
        }

        private void CargarCombos()
        {
            Elementos = new ObservableCollection<Elemento>(_elementoService.ObtenerElementos());
            Balances = new ObservableCollection<Balance>(_balanceService.ObtenerBalances());
        }
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
            CargarAutomatizacion(CuentaSeleccionada.IdPlanCuenta);

        }

        // =========================
        // CRUD
        // =========================

        private void Agregar()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Codigo))
                {
                    MessageBox.Show("Ingresa el código.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Descripcion))
                {
                    MessageBox.Show("Ingresa la descripción.");
                    return;
                }

                if (ElementoSeleccionado == null)
                {
                    MessageBox.Show("Selecciona el elemento.");
                    return;
                }

                if (_service.ExisteCodigo(Codigo))
                {
                    MessageBox.Show("Ya existe una cuenta con ese código.");
                    return;
                }

                var entity = new PlanCuenta
                {
                    Codigo = Codigo,
                    Descripcion = Descripcion,
                    Nivel = Nivel,
                    CodigoPadre = string.IsNullOrWhiteSpace(CodigoPadre) ? null : CodigoPadre,
                    IdElemento = ElementoSeleccionado.IdElemento,
                    IdBalance = BalanceSeleccionado?.IdBalance,
                    Analisis = Analisis
                };

                var nuevaCuenta = _service.InsertarCuenta(entity);

                if (nuevaCuenta == null || nuevaCuenta.IdPlanCuenta == 0)
                {
                    MessageBox.Show("Error: la cuenta no fue devuelta por el servicio.");
                    return;
                }

                // ✅ Recargar desde BD (garantiza consistencia total)
                CargarDatos();

                // ✅ Seleccionar la cuenta recién insertada
                CuentaSeleccionada = PlanCuentas.FirstOrDefault(x => x.IdPlanCuenta == nuevaCuenta.IdPlanCuenta);

                MessageBox.Show("Cuenta agregada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
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
                var entity = new PlanCuenta
                {
                    IdPlanCuenta = CuentaSeleccionada.IdPlanCuenta,
                    Codigo = Codigo,
                    Descripcion = Descripcion,
                    Nivel = Nivel,
                    CodigoPadre = CodigoPadre,
                    IdElemento = ElementoSeleccionado.IdElemento,
                    IdBalance = BalanceSeleccionado?.IdBalance,
                    Analisis = Analisis
                };

                int idModificado = CuentaSeleccionada.IdPlanCuenta;

                _service.ModificarCuenta(entity);

                // ✅ Recargar desde BD (misma solución que Agregar)
                CargarDatos();

                // ✅ Volver a seleccionar la cuenta modificada
                CuentaSeleccionada = PlanCuentas.FirstOrDefault(x => x.IdPlanCuenta == idModificado);


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

                PlanCuentas.Remove(CuentaSeleccionada);
                CuentasPadre.Remove(CuentaSeleccionada);

                Limpiar();

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


        private void CargarAutomatizacion(int idPlanCuenta)
        {
            var result = _automatizacionService.Obtener(idPlanCuenta);

            Automatizacion = new ObservableCollection<AutomatizacionDetalleItem>(
                result.lista.Select(AutomatizacionMapper.ToItem)
            );

            TieneAutomatizacion = result.estado;

            OnPropertyChanged(nameof(Automatizacion));
            OnPropertyChanged(nameof(TieneAutomatizacion));
        }
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
                MessageBox.Show("Selecciona una cuenta primero.");
                return;
            }

            if (!TieneAutomatizacion)
            {
                MessageBox.Show("Activa los asientos automáticos primero.");
                return;
            }

            var vm = new AgregarAsientoViewModel(CuentaSeleccionada.IdEmpresa);

            var view = new Views.PlanCuentas.AgregarAsientoView
            {
                DataContext = vm
            };

            // 🔥 AQUÍ VA
            vm.CloseAction = result =>
            {
                view.DialogResult = result;
                view.Close();
            };

            var result = view.ShowDialog();

            if (result == true && vm.Resultado != null)
            {
                Automatizacion.Add(vm.Resultado);
            }


        }

        private void AsientoMenos()
        {
            try
            {
                if (CuentaSeleccionada == null)
                {
                    MessageBox.Show("Selecciona una cuenta primero.");
                    return;
                }

                if (AutomatizacionSeleccionada == null)
                {
                    MessageBox.Show("Selecciona una fila de automatización.");
                    return;
                }

                Automatizacion.Remove(AutomatizacionSeleccionada);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al quitar asiento: " + ex.Message);
            }
        }

        private void guardarautomatizacion()
        {
            try
            {
                if (CuentaSeleccionada == null)
                {
                    MessageBox.Show("Selecciona una cuenta.");
                    return;
                }

                // 🔴 DESACTIVAR
                if (!TieneAutomatizacion)
                {
                    _automatizacionService.Guardar(
                        CuentaSeleccionada.IdPlanCuenta,
                        new List<CuentaAutomatizacionDetalle>(),
                        false
                    );

                    MessageBox.Show("Automatización desactivada.");
                    return;
                }

                var detalles = Automatizacion
                    .Select(AutomatizacionMapper.ToEntity)
                    .ToList();

                // 🔥 VALIDACIÓN AHORA EN SERVICE
                _automatizacionService.Validar(detalles);

                _automatizacionService.Guardar(
                    CuentaSeleccionada.IdPlanCuenta,
                    detalles,
                    true
                );

                MessageBox.Show("Automatización guardada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
