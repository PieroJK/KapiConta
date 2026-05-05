using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Views.GestionAsiento;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class AsientoManualViewModel : INotifyPropertyChanged
    {
        private readonly MesService _mesService;
        private readonly SubDiarioService _subDiarioService;
        private readonly LibroService _libroService;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // COLECCIONES (COMBOS)
        // =========================

        public ObservableCollection<Mes> Meses { get; set; }
        public ObservableCollection<SubDiario> SubDiarios { get; set; }
        public ObservableCollection<Libro> Libros { get; set; }

        public ObservableCollection<string> Monedas { get; set; } = new() { "PEN", "USD" };

        public ObservableCollection<AsientoDetalle> Detalle { get; set; }

        // =========================
        // COMMANDS (BOTONES)
        // =========================
        public ICommand AgregarDetalleAsientoCommand { get; set; }

        public ICommand GuardarCommand { get; set; }

        // =========================
        // SELECCIONES
        // =========================

        private Mes _mesSeleccionado;
        public Mes MesSeleccionado
        {
            get => _mesSeleccionado;
            set
            {
                _mesSeleccionado = value;
                OnPropertyChanged();
                GenerarReferenciaTemporal();
            }
        }

        private SubDiario _subDiarioSeleccionado;
        public SubDiario SubDiarioSeleccionado
        {
            get => _subDiarioSeleccionado;
            set
            {
                _subDiarioSeleccionado = value;
                OnPropertyChanged();

                // 🔥 llenar nombre automáticamente
                SubDiarioNombre = value?.Nombre ?? "";

                GenerarReferenciaTemporal();
            }
        }

        private Libro _libroSeleccionado;
        public Libro LibroSeleccionado
        {
            get => _libroSeleccionado;
            set
            {
                _libroSeleccionado = value;
                OnPropertyChanged();

                // 🔥 llenar nombre automáticamente
                LibroNombre = value?.Nombre ?? "";
            }
        }

        private DateTime? _fecha;
        public DateTime? Fecha
        {
            get => _fecha;
            set { _fecha = value; OnPropertyChanged(); }
        }

        private DateTime? _fechaVencimiento;
        public DateTime? FechaVencimiento
        {
            get => _fechaVencimiento;
            set { _fechaVencimiento = value; OnPropertyChanged(); }
        }

        private string _monedaSeleccionada = "PEN";
        public string MonedaSeleccionada
        {
            get => _monedaSeleccionada;
            set { _monedaSeleccionada = value; OnPropertyChanged(); }
        }
        // =========================
        // TEXTOS (TEXTBOX)
        // =========================

        private string _subDiarioNombre;
        public string SubDiarioNombre
        {
            get => _subDiarioNombre;
            set { _subDiarioNombre = value; OnPropertyChanged(); }
        }

        private string _libroNombre;
        public string LibroNombre
        {
            get => _libroNombre;
            set { _libroNombre = value; OnPropertyChanged(); }
        }

        private string _referencia;
        public string Referencia
        {
            get => _referencia;
            set { _referencia = value; OnPropertyChanged(); }
        }

        // =========================
        // CONSTRUCTOR
        // =========================

        public AsientoManualViewModel()
        {
            AgregarDetalleAsientoCommand = new RelayCommand(AbrirAgregarDetalleAsiento);
            GuardarCommand = new RelayCommand(GuardarAsiento);

            _mesService = new MesService();
            _subDiarioService = new SubDiarioService();
            _libroService = new LibroService();

            Detalle = new ObservableCollection<AsientoDetalle>();

            CargarDatos();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarDatos()
        {
            Meses = new ObservableCollection<Mes>(_mesService.ObtenerMeses());
            SubDiarios = new ObservableCollection<SubDiario>(_subDiarioService.ObtenerSubDiarios());
            Libros = new ObservableCollection<Libro>(_libroService.Listar());

            OnPropertyChanged(nameof(Meses));
            OnPropertyChanged(nameof(SubDiarios));
            OnPropertyChanged(nameof(Libros));
        }

        // =========================
        // METODOS TEXFIELD
        // =========================

        private void GenerarReferenciaTemporal()
        {
            try
            {
                if (MesSeleccionado == null)
                    return;

                if (SubDiarioSeleccionado == null)
                    return;

                if (Session.CurrentEmpresa == null)
                    return;

                var service = new AsientoService();

                Referencia = service.ObtenerSiguienteReferencia(
                    Session.CurrentEmpresa.IdEmpresa,
                    MesSeleccionado.IdMes,
                    SubDiarioSeleccionado.IdSubDiario,
                    SubDiarioSeleccionado.Diario,   // 👈 antes Codigo
                    MesSeleccionado.mes            // 👈 antes Codigo
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al generar la referencia del asiento.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ===========================    
        // METODOS BOTONES INFERIORES
        // ===========================

        private void AbrirAgregarDetalleAsiento()
        {
            try
            {
                if (Session.CurrentEmpresa == null) return;

                // ✅ pasa la moneda real de la cabecera cuando la tengas
                string moneda = "PEN"; // 🔜 reemplazar por MonedaSeleccionada cuando esté disponible

                var vm = new AgregarDetalleAsientoViewModel(moneda);

                var window = new AgregarDetalleAsientoWindow
                {
                    DataContext = vm,
                    Owner = Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(w => w.IsActive)
                };

                vm.OnAgregar = item => AgregarLineaDesdeResultado(item);
                vm.Cerrar = () => window.Close();

                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir ventana: " + ex.Message);
            }
        }

        private void AgregarLineaDesdeResultado(AsientoDetalle item)
        {
            // si quieres mantener automatización:
            bool usarAuto = item.PlanCuenta?.TieneAutomatizacion ?? false;

            if (usarAuto)
                AgregarLineasAutomatizadas(item);
            else
                AgregarLineaSimple(item);

            RecalcularTotales();
        }

        private void AgregarLineaSimple(AsientoDetalle item)
        {
            Detalle.Add(item);
        }

        private void AgregarLineasAutomatizadas(AsientoDetalle itemBase)
        {
            var service = new CuentaAutomatizacionDetalleService();
            var detalles = service.Listar(itemBase.IdPlanCuenta);

            if (detalles.Count == 0)
            {
                AgregarLineaSimple(itemBase);
                return;
            }

            decimal importeBase = itemBase.Debe > 0 ? itemBase.Debe : itemBase.Haber;
            bool invertir = itemBase.Haber > 0;

            foreach (var det in detalles)
            {
                bool esDebe = det.TipoMovimiento?.Trim().ToUpper() == "D";
                decimal importeCalculado = Math.Round(importeBase * (det.Porcentaje / 100m), 2);

                // ✅ lógica corregida
                bool vaAlDebe = invertir ? !esDebe : esDebe;

                Detalle.Add(new AsientoDetalle
                {
                    IdPlanCuenta = det.IdCuentaRelacionada,
                    PlanCuenta = det.CuentaRelacionada, // ✅ ya viene del mapper
                    Moneda = itemBase.Moneda,
                    Debe = vaAlDebe ? importeCalculado : 0m,
                    Haber = vaAlDebe ? 0m : importeCalculado,
                    Glosa = itemBase.Glosa,

                    // ✅ propagar datos del comprobante
                    IdTipoFacturacion = itemBase.IdTipoFacturacion,
                    TipoFacturacion = itemBase.TipoFacturacion,
                    IdTercero = itemBase.IdTercero,
                    Tercero = itemBase.Tercero,
                    SerieComprobante = itemBase.SerieComprobante,
                    IdTipoOperacion = itemBase.IdTipoOperacion
                });
            }
        }

        private void RecalcularTotales()
        {
            decimal totalDebe = Detalle.Sum(x => x.Debe);
            decimal totalHaber = Detalle.Sum(x => x.Haber);

            // luego los bindeas si quieres
        }

        private (Asiento asiento, List<AsientoDetalle> detalles)? ConstruirModelo()
        {
            // =========================
            // VALIDACIONES
            // =========================

            if (MesSeleccionado == null)
            {
                MessageBox.Show("Selecciona un mes.");
                return null;
            }

            if (SubDiarioSeleccionado == null)
            {
                MessageBox.Show("Selecciona un sub-diario.");
                return null;
            }

            if (LibroSeleccionado == null)
            {
                MessageBox.Show("Selecciona un libro.");
                return null;
            }

            // ⚠️ OJO: aquí no tienes Fecha en VM → debes crearla
            DateTime fecha = DateTime.Now; // 🔴 luego lo bindeas desde DatePicker

            if (Detalle.Count == 0)
            {
                MessageBox.Show("Debes agregar al menos una línea.");
                return null;
            }

            decimal totalDebe = Detalle.Sum(x => x.Debe);
            decimal totalHaber = Detalle.Sum(x => x.Haber);

            if (Math.Round(totalDebe, 2) != Math.Round(totalHaber, 2))
            {
                MessageBox.Show("Debe y Haber no cuadran.");
                return null;
            }

            // =========================
            // CABECERA
            // =========================

            var asiento = new Asiento
            {
                IdEmpresa = Session.CurrentEmpresa.IdEmpresa,
                IdPeriodo = Session.CurrentPeriodo.IdPeriodo,
                IdMes = MesSeleccionado.IdMes,
                IdSubDiario = SubDiarioSeleccionado.IdSubDiario,
                IdLibro = LibroSeleccionado.IdLibro,
                Referencia = Referencia,
                Fecha = Fecha.Value,
                Moneda = MonedaSeleccionada, // 🔜 luego bindear combo
                IdTipoCambio = null,
                FechaVen = FechaVencimiento, // 🔜 bindear DatePicker
                IdUsuario = Session.CurrentUser.Id
            };

            // =========================
            // DETALLE
            // =========================

            var detalles = Detalle.Select(x => new AsientoDetalle
            {
                IdPlanCuenta = x.IdPlanCuenta,
                Moneda = x.Moneda,
                Debe = x.Debe,
                Haber = x.Haber,
                IdTipoFacturacion = x.IdTipoFacturacion,
                IdTercero = x.IdTercero,
                SerieComprobante = x.SerieComprobante,
                Glosa = x.Glosa,
                IdTipoOperacion = x.IdTipoOperacion,
                IdCosto = x.IdCosto
            }).ToList();

            return (asiento, detalles);
        }

        private string GenerarSiguienteReferenciaDesdeActual(string referenciaActual)
        {
            if (string.IsNullOrWhiteSpace(referenciaActual) || referenciaActual.Length < 9)
                return referenciaActual;

            string prefijo = referenciaActual.Substring(0, 3);
            string numeroTexto = referenciaActual.Substring(3);

            if (!int.TryParse(numeroTexto, out int numero))
                return referenciaActual;

            int siguiente = numero + 1;

            return $"{prefijo}{siguiente:000000}";
        }

        private int? _idAsientoActual;

        private void GuardarAsiento()
        {
            try
            {
                // =========================
                // VALIDAR SI YA EXISTE
                // =========================
                if (_idAsientoActual.HasValue)
                {
                    MessageBox.Show(
                        "Este asiento ya existe. Usa Modificar si deseas actualizarlo.",
                        "Asientos",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // =========================
                // CONSTRUIR MODELO
                // =========================
                var data = ConstruirModelo();

                if (data == null)
                    return;

                var (asiento, detalles) = data.Value;

                // =========================
                // GUARDAR
                // =========================
                var service = new AsientoService();

                int idAsiento = service.GuardarAsiento(asiento, detalles);

                MessageBox.Show(
                    $"Asiento guardado correctamente. ID: {idAsiento}",
                    "Asientos",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // =========================
                // MANEJO DE REFERENCIA
                // =========================

                string referenciaActual = Referencia;

                // limpiar pantalla
                Limpiar();

                // generar siguiente correlativo
                Referencia = GenerarSiguienteReferenciaDesdeActual(referenciaActual);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar asiento.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Limpiar()
        {
            // =========================
            // COMBOS (selecciones)
            // =========================
            MesSeleccionado = null;
            SubDiarioSeleccionado = null;
            LibroSeleccionado = null;

            // =========================
            // FECHAS
            // =========================
            Fecha = null;
            FechaVencimiento = null;

            // =========================
            // TEXTOS
            // =========================
            SubDiarioNombre = string.Empty;
            LibroNombre = string.Empty;
            Referencia = string.Empty;

            // =========================
            // MONEDA (opcional)
            // =========================
            MonedaSeleccionada = "PEN"; // o déjalo como estaba si no quieres resetearlo

            // =========================
            // DETALLE (GRID)
            // =========================
            Detalle.Clear();

            // =========================
            // ESTADO INTERNO
            // =========================
            _idAsientoActual = null;
        }
    }
}
