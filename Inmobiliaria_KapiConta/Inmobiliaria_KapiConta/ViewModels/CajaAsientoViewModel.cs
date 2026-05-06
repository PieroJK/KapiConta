using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class CajaAsientoViewModel : INotifyPropertyChanged
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

        private readonly ObservableCollection<AsientoDetalle> _detalle;
        public Action<bool> CerrarVentana { get; set; } // true = DialogResult = true

        // =========================
        // COLECCIONES
        // =========================

        private ObservableCollection<PlanCuenta> _cuentas;
        public ObservableCollection<PlanCuenta> Cuentas
        {
            get => _cuentas;
            set { _cuentas = value; OnPropertyChanged(); }
        }

        // =========================
        // SELECCIONES
        // =========================

        private PlanCuenta _cuentaSeleccionada;
        public PlanCuenta CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set { _cuentaSeleccionada = value; OnPropertyChanged(); }
        }

        // =========================
        // TEXTOS
        // =========================

        private string _glosa;
        public string Glosa
        {
            get => _glosa;
            set { _glosa = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand AceptarCommand { get; }
        public ICommand CancelarCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public CajaAsientoViewModel(ObservableCollection<AsientoDetalle> detalle)
        {
            _detalle = detalle;

            AceptarCommand = new RelayCommand(Aceptar);
            CancelarCommand = new RelayCommand(Cancelar);

            CargarCuentas();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarCuentas()
        {
            var service = new PlanCuentasService(Session.CurrentEmpresa.IdEmpresa);
            Cuentas = new ObservableCollection<PlanCuenta>(service.ListarCajaBancos());
        }

        // =========================
        // MÉTODOS
        // =========================

        private void Aceptar()
        {
            // --- VALIDACIONES ---
            if (CuentaSeleccionada == null)
            {
                MessageBox.Show("Seleccione una cuenta.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Glosa))
            {
                MessageBox.Show("Ingrese la glosa.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // --- CALCULAR DIFERENCIA ---
            decimal totalDebe = _detalle.Sum(x => x.Debe);
            decimal totalHaber = _detalle.Sum(x => x.Haber);
            decimal diferencia = totalDebe - totalHaber;

            if (Math.Round(diferencia, 2) == 0)
            {
                MessageBox.Show("El asiento ya está cuadrado.", "Asientos",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // --- ELIMINAR AJUSTES ANTERIORES ---
            var ajustes = _detalle
                .Where(x => x.Glosa == "Ajuste por diferencia")
                .ToList();

            foreach (var item in ajustes)
                _detalle.Remove(item);

            // --- AGREGAR LÍNEA DE AJUSTE ---
            bool vaAlDebe = diferencia < 0;
            decimal monto = Math.Abs(diferencia);

            _detalle.Add(new AsientoDetalle
            {
                IdPlanCuenta = CuentaSeleccionada.IdPlanCuenta,
                PlanCuenta = CuentaSeleccionada,
                Moneda = "PEN",
                Debe = vaAlDebe ? monto : 0m,
                Haber = vaAlDebe ? 0m : monto,
                Glosa = Glosa
            });

            // --- CERRAR CON ÉXITO ---
            CerrarVentana?.Invoke(true);
        }

        private void Cancelar()
        {
            CerrarVentana?.Invoke(false);
        }
    }
}
