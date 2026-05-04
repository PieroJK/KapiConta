using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class SelectorCuentaItem
    {
        public string CodigoCuenta { get; set; } = string.Empty;
        public string NombreCuenta { get; set; } = string.Empty;
    }

    public class SelectorCuentaViewModel : INotifyPropertyChanged
    {
        private readonly PlanCuentasService _service;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

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
                CollectionViewSource.GetDefaultView(Cuentas)?.Refresh();
            }
        }

        // =========================
        // DATOS
        // =========================

        private ObservableCollection<SelectorCuentaItem> _cuentas;
        public ObservableCollection<SelectorCuentaItem> Cuentas
        {
            get => _cuentas;
            set { _cuentas = value; OnPropertyChanged(); }
        }

        private SelectorCuentaItem _cuentaSeleccionada;
        public SelectorCuentaItem CuentaSeleccionada
        {
            get => _cuentaSeleccionada;
            set { _cuentaSeleccionada = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand SeleccionarCommand { get; }

        // =========================
        // RESULTADO
        // =========================

        public Action<SelectorCuentaItem> OnCuentaSeleccionada { get; set; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public SelectorCuentaViewModel()
        {
            _service = new PlanCuentasService(Session.CurrentEmpresa.IdEmpresa);

            SeleccionarCommand = new RelayCommand(Seleccionar);

            CargarCuentas();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarCuentas()
        {
            var lista = _service.ObtenerPlanCuentas();

            Cuentas = new ObservableCollection<SelectorCuentaItem>(
                lista.Select(x => new SelectorCuentaItem
                {
                    CodigoCuenta = x.Codigo,
                    NombreCuenta = x.Descripcion
                })
            );

            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var view = CollectionViewSource.GetDefaultView(Cuentas);
            view.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(TextoBusqueda)) return true;
                var cuenta = item as SelectorCuentaItem;
                return cuenta.CodigoCuenta.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                    || cuenta.NombreCuenta.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase);
            };
        }

        // =========================
        // SELECCIONAR (pendiente)
        // =========================

        private void Seleccionar()
        {
            if (CuentaSeleccionada == null) return;

            OnCuentaSeleccionada?.Invoke(CuentaSeleccionada);
        }
    }
}
