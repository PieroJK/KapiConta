using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Views.GestionAsiento;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
        // =========================
        // FLAG INTERNO
        // =========================
        private bool _limpiando = false;

        private bool _guardando = false;

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

        public ICommand BorrarTodoCommand { get; set; }

        public ICommand AbrirCajaAsientoCommand { get; set; }

        public ICommand AbrirPendientesCommand { get; set; }

        public ICommand RevertirDetalleCommand { get; set; }

        public ICommand AplicarDiferenciaCommand { get; set; }
        public ICommand QuitarDetalleCommand { get; set; }

        // =========================
        // SELECCIONES
        // =========================

        // Propiedad para la fila seleccionada — en la sección SELECCIONES
        private AsientoDetalle _detalleSeleccionado;
        public AsientoDetalle DetalleSeleccionado
        {
            get => _detalleSeleccionado;
            set { _detalleSeleccionado = value; OnPropertyChanged(); }
        }

        private Mes _mesSeleccionado;
        public Mes MesSeleccionado
        {
            get => _mesSeleccionado;
            set
            {
                _mesSeleccionado = value;
                OnPropertyChanged();

                if (!_limpiando)          // 👈 solo si NO estamos limpiando
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

                if (!_limpiando)          // 👈 solo si NO estamos limpiando
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

        private string _totalDebe = "0.00";
        public string TotalDebe
        {
            get => _totalDebe;
            set { _totalDebe = value; OnPropertyChanged(); }
        }

        private string _totalHaber = "0.00";
        public string TotalHaber
        {
            get => _totalHaber;
            set { _totalHaber = value; OnPropertyChanged(); }
        }

        private string _diferencia = "0.00";
        public string Diferencia
        {
            get => _diferencia;
            set { _diferencia = value; OnPropertyChanged(); }
        }

        private string _fechaEstado = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        public string FechaEstado
        {
            get => _fechaEstado;
            set { _fechaEstado = value; OnPropertyChanged(); }
        }

        // =========================
        // CONSTRUCTOR
        // =========================

        public AsientoManualViewModel()
        {
            AgregarDetalleAsientoCommand = new RelayCommand(AbrirAgregarDetalleAsiento);
            GuardarCommand = new RelayCommand(GuardarAsiento);
            BorrarTodoCommand = new RelayCommand(BorrarTodo);
            AbrirCajaAsientoCommand = new RelayCommand(AbrirCajaAsiento);
            AbrirPendientesCommand = new RelayCommand(AbrirPendientes);
            QuitarDetalleCommand = new RelayCommand(QuitarDetalle);

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
            RevertirDetalleCommand = new RelayCommand(RevertirDetalle);
            AplicarDiferenciaCommand = new RelayCommand(AplicarDiferencia);

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
        private void QuitarDetalle()
        {
            if (DetalleSeleccionado == null)
            {
                MessageBox.Show("Selecciona una fila para quitar.", "Asientos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Detalle.Remove(DetalleSeleccionado);
            DetalleSeleccionado = null;

            RecalcularTotales();
        }
        private void AplicarDiferencia()
        {
            decimal totalDebe = Detalle.Sum(x => x.Debe);
            decimal totalHaber = Detalle.Sum(x => x.Haber);
            decimal diferencia = totalDebe - totalHaber;

            if (Math.Round(diferencia, 2) == 0)
            {
                MessageBox.Show("El asiento ya está cuadrado.", "Asientos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // =========================
            // ELIMINAR AJUSTES ANTERIORES
            // =========================
            var ajustes = Detalle
                .Where(x => x.Glosa == "Ajuste por diferencia")
                .ToList();

            foreach (var item in ajustes)
                Detalle.Remove(item);

            // =========================
            // CALCULAR CUENTA Y MONTO
            // =========================
            bool vaAlDebe = diferencia < 0;
            decimal monto = Math.Abs(diferencia);
            string cuentaCodigo = vaAlDebe ? "676" : "776";

            // =========================
            // BUSCAR CUENTA EN BD
            // =========================
            var service = new PlanCuentasService(Session.CurrentEmpresa.IdEmpresa);
            var cuenta = service.ObtenerPorCodigo(cuentaCodigo);

            if (cuenta == null)
            {
                MessageBox.Show($"No se encontró la cuenta {cuentaCodigo}.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // =========================
            // AGREGAR LÍNEA DE AJUSTE
            // =========================
            Detalle.Add(new AsientoDetalle
            {
                IdPlanCuenta = cuenta.IdPlanCuenta,
                PlanCuenta = cuenta,
                Moneda = MonedaSeleccionada,
                Debe = vaAlDebe ? monto : 0m,
                Haber = vaAlDebe ? 0m : monto,
                Glosa = "Ajuste por diferencia"
            });

            // =========================
            // ACTUALIZAR TOTALES Y SELECCIÓN
            // =========================
            RecalcularTotales();

            // 🔥 seleccionar última fila (equivalente al ScrollIntoView del behind)
            DetalleSeleccionado = Detalle.Last();
        }
        private void RevertirDetalle()
        {
            if (DetalleSeleccionado == null)
            {
                MessageBox.Show("Selecciona una fila para revertir.", "Asientos",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal debeOriginal = DetalleSeleccionado.Debe;
            decimal haberOriginal = DetalleSeleccionado.Haber;

            DetalleSeleccionado.Debe = haberOriginal;
            DetalleSeleccionado.Haber = debeOriginal;

            // 🔥 forzar refresco visual del DataGrid
            var index = Detalle.IndexOf(DetalleSeleccionado);
            Detalle[index] = DetalleSeleccionado;

            RecalcularTotales();
        }
        private void AbrirPendientes()
        {
            try
            {
                if (Session.CurrentEmpresa == null)
                    return;

                var service = new PendienteService();
                var datos = service.ObtenerAnalisisPendientes(Session.CurrentEmpresa.IdEmpresa);

                if (datos == null || datos.Count == 0)
                {
                    MessageBox.Show(
                        "No hay documentos con Análisis = True para esta empresa.",
                        "Pendientes",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var ventana = new SelectorPendientesWindow(datos)
                {
                    Owner = Application.Current.Windows
                        .OfType<Window>()
                        .FirstOrDefault(w => w.IsActive)
                };

                if (ventana.ShowDialog() == true)
                {
                    foreach (var p in ventana.Resultados)
                    {
                        var nuevoItem = new AsientoDetalle
                        {
                            // =========================
                            // IDs
                            // =========================
                            IdPlanCuenta = p.IdPlanCuenta,
                            IdTercero = p.IdTercero,

                            // =========================
                            // OBJETOS DE NAVEGACIÓN
                            // =========================
                            PlanCuenta = new PlanCuenta
                            {
                                IdPlanCuenta = p.IdPlanCuenta,
                                Codigo = p.Cuenta,
                            },

                            Tercero = new Tercero
                            {
                                IdTercero = p.IdTercero ?? 0,
                                Documento = p.Ruc,
                                RazonSocial = p.Proveedor,
                            },

                            // =========================
                            // DATOS DEL COMPROBANTE
                            // =========================
                            SerieComprobante = p.Documento,

                            // =========================
                            // GLOSA Y MONTOS
                            // =========================
                            Glosa = "CANC. DOC " + p.Documento,
                            Debe = p.Saldo < 0 ? Math.Abs(p.Saldo) : 0m,
                            Haber = p.Saldo < 0 ? 0m : p.Saldo,
                        };

                        Detalle.Add(nuevoItem);
                    }

                    RecalcularTotales();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al abrir pendientes.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void BorrarTodo()
        {
            if (Detalle.Count == 0)
                return;

            var result = MessageBox.Show(
                "¿Deseas borrar todas las líneas del detalle?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Detalle.Clear();
                _idAsientoActual = null;
                RecalcularTotales();
                GenerarReferenciaTemporal();
            }
        }

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

        // ===========================    
        // METODOS AUXILIARES
        // ===========================
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

        // Reemplaza el método existente
        private void RecalcularTotales()
        {
            decimal totalDebe = Detalle.Sum(x => x.Debe);
            decimal totalHaber = Detalle.Sum(x => x.Haber);
            decimal diferencia = totalDebe - totalHaber;

            TotalDebe = totalDebe.ToString("N2", CultureInfo.InvariantCulture);
            TotalHaber = totalHaber.ToString("N2", CultureInfo.InvariantCulture);
            Diferencia = diferencia.ToString("N2", CultureInfo.InvariantCulture);
            FechaEstado = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
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

        // ===========================    
        // METODOS BOTONES DERECHA
        // ===========================
        private int _contadorGuardado = 0;
        private void GuardarAsiento()
        {
            _contadorGuardado++;

            MessageBox.Show(
                $"Guardar llamado {_contadorGuardado} veces",
                "DEBUG",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            Debug.WriteLine($"[GuardarAsiento] llamado — StackTrace:");
            Debug.WriteLine(new System.Diagnostics.StackTrace().ToString());
            if (_guardando)
                return;

            _guardando = true;
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
            finally
            {
                _guardando = false;
            }
        }
        private void AbrirCajaAsiento()
        {
            // ✅ elimina la línea del VM que no se usa
            // var vm = new CajaAsientoViewModel(Detalle);  👈 ELIMINAR

            var ventana = new CajaAsientoWindow(Detalle)
            {
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)
            };

            if (ventana.ShowDialog() == true)
            {
                RecalcularTotales();
            }
        }

        // =========================
        // LIMPIAR PANTALLA
        // =========================
        // Limpiar con flag activado
        private void Limpiar()
        {
            _limpiando = true;            // 🔒 bloquear efectos secundarios

            // COMBOS
            MesSeleccionado = null;
            SubDiarioSeleccionado = null;
            LibroSeleccionado = null;

            // FECHAS
            Fecha = null;
            FechaVencimiento = null;

            // TEXTOS
            SubDiarioNombre = string.Empty;
            LibroNombre = string.Empty;
            Referencia = string.Empty;

            // MONEDA
            MonedaSeleccionada = "PEN";

            // DETALLE
            Detalle.Clear();

            // ESTADO INTERNO
            _idAsientoActual = null;

            _limpiando = false;           // 🔓 desbloquear efectos secundarios
        }
    }
}
