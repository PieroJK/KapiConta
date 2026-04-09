using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Interfaces;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class SeleccionPeriodoViewModel : INotifyPropertyChanged, IResizableView
    {
        private readonly MainViewModel _mainVM;
        private readonly PeriodoService _periodoService;

        public SeleccionPeriodoViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            _periodoService = new PeriodoService();

            ContinuarCommand = new RelayCommand(Continuar);
            VolverCommand = new RelayCommand(Volver);

            CargarPeriodos();
        }

        public double Width => 900;
        public double Height => 600;

        public ObservableCollection<Periodo> Periodos { get; set; } = new();

        private Periodo _periodoSeleccionado;
        public Periodo PeriodoSeleccionado
        {
            get => _periodoSeleccionado;
            set
            {
                _periodoSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public ICommand ContinuarCommand { get; }

        public ICommand VolverCommand { get; }

        private void CargarPeriodos()
        {
            if (Session.CurrentEmpresa == null)
                return;

            var lista = _periodoService.ObtenerPeriodos(Session.CurrentEmpresa.IdEmpresa);

            Periodos.Clear();
            foreach (var p in lista)
                Periodos.Add(p);

            if (Periodos.Count > 0)
                PeriodoSeleccionado = Periodos[0];
        }

        private void Continuar()
        {
            if (PeriodoSeleccionado == null)
                return;

            // ✅ Guardar en sesión
            Session.CurrentPeriodo = PeriodoSeleccionado;

            // 👉 Ir al dashboard
            _mainVM.Navigation.NavigateTo(new DashboardViewModel(_mainVM));
        }

        private void Volver()
        {
            Session.CurrentEmpresa = null;
            Session.CurrentPeriodo = null;

            // 🔙 Navegar a login
            _mainVM.Navigation.NavigateTo(new SeleccionEmpresaViewModel(_mainVM));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
