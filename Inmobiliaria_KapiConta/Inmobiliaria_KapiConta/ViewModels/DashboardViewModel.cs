using Inmobiliaria_KapiConta.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Interfaces;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged, IResizableView
    {
        private readonly MainViewModel _mainVM;
        public double Width => 1500;   // puedes ajustarlo
        public double Height => 800;
        public string Usuario => $"Usuario: {Session.CurrentUser?.Username}";
        public string Empresa => $"Empresa: {Session.CurrentEmpresa?.Nombre}";
        public string Periodo => $"Periodo: {Session.CurrentPeriodo?.Anio}";

        public ICommand PlanCuentasCommand { get; }
        public ICommand RegistrarTercerosCommand { get; }
        public ICommand ListadoTercerosCommand { get; }

        private object _contenidoActual;
        public object ContenidoActual
        {
            get => _contenidoActual;
            set
            {
                _contenidoActual = value;
                OnPropertyChanged();
            }
        }
        public DashboardViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;

            PlanCuentasCommand = new RelayCommand(() =>
            {
                if (Session.CurrentEmpresa == null) return;

                ContenidoActual = new PlanCuentasViewModel(Session.CurrentEmpresa.IdEmpresa);
            });

            RegistrarTercerosCommand = new RelayCommand(() =>
                MessageBox.Show("Módulo Registrar Terceros aún no implementado"));

            ListadoTercerosCommand = new RelayCommand(() =>
                MessageBox.Show("Módulo Listado de Terceros aún no implementado"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}