using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class AgregarAsientoViewModel : INotifyPropertyChanged
    {
        private readonly PlanCuentasService _service;

        public ObservableCollection<PlanCuenta> Cuentas { get; set; }
        private ObservableCollection<PlanCuenta> _cuentasOriginales;

        private PlanCuenta _cuentaSeleccionada;
        public PlanCuenta CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set
            {
                _cuentaSeleccionada = value;
                OnPropertyChanged();
                AceptarCommand.RaiseCanExecuteChanged();
            }
        }

        private string _buscar;
        public string Buscar
        {
            get => _buscar;
            set
            {
                _buscar = value;
                OnPropertyChanged();
                Filtrar();
            }
        }

        private string _tipo = "D"; // D = Debe
        public string Tipo
        {
            get => _tipo;
            set
            {
                _tipo = value;
                OnPropertyChanged();
            }
        }

        private string _porcentaje = "100";
        public string Porcentaje
        {
            get => _porcentaje;
            set
            {
                _porcentaje = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AceptarCommand { get; }
        public RelayCommand CancelarCommand { get; }

        public AutomatizacionDetalleItem Resultado { get; private set; }

        public AgregarAsientoViewModel(int empresaId)
        {
            _service = new PlanCuentasService(empresaId);

            AceptarCommand = new RelayCommand(Aceptar, PuedeAceptar);
            CancelarCommand = new RelayCommand(Cancelar);

            CargarCuentas();
        }

        private void CargarCuentas()
        {
            var lista = _service.ObtenerPlanCuentas();

            _cuentasOriginales = new ObservableCollection<PlanCuenta>(lista);
            Cuentas = new ObservableCollection<PlanCuenta>(lista);

            OnPropertyChanged(nameof(Cuentas));
        }

        private void Filtrar()
        {
            if (string.IsNullOrWhiteSpace(Buscar))
            {
                Cuentas = new ObservableCollection<PlanCuenta>(_cuentasOriginales);
            }
            else
            {
                var filtro = Buscar.ToLower();

                var lista = _cuentasOriginales
                    .Where(x =>
                        (!string.IsNullOrWhiteSpace(x.Codigo) && x.Codigo.ToLower().Contains(filtro)) ||
                        (!string.IsNullOrWhiteSpace(x.Descripcion) && x.Descripcion.ToLower().Contains(filtro)))
                    .ToList();

                Cuentas = new ObservableCollection<PlanCuenta>(lista);
            }

            OnPropertyChanged(nameof(Cuentas));
        }

        private bool PuedeAceptar()
        {
            return CuentaSeleccionada != null;
        }

        private void Aceptar()
        {
            if (!decimal.TryParse(Porcentaje, out decimal porcentaje) || porcentaje <= 0)
            {
                MessageBox.Show("Ingresa un porcentaje válido.");
                return;
            }

            Resultado = new AutomatizacionDetalleItem
            {
                IdCuentaRelacionada = CuentaSeleccionada.IdPlanCuenta,
                CuentaCodigo = CuentaSeleccionada.Codigo,
                CuentaDescripcion = CuentaSeleccionada.Descripcion,
                TipoMovimiento = Tipo,
                Porcentaje = porcentaje
            };

            CerrarVentana(true);
        }

        private void Cancelar()
        {
            CerrarVentana(false);
        }

        // 🔥 CLAVE PARA CERRAR DESDE VM
        public Action<bool?> CloseAction { get; set; }

        private void CerrarVentana(bool? result)
        {
            CloseAction?.Invoke(result);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}