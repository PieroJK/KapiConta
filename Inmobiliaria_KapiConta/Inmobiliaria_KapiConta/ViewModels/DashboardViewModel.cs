using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

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
        //ventanas
        public ObservableCollection<TabItemViewModel> Tabs { get; set; }
        = new ObservableCollection<TabItemViewModel>();

        private TabItemViewModel _tabSeleccionado;
        public TabItemViewModel TabSeleccionado
        {
            get => _tabSeleccionado;
            set
            {
                _tabSeleccionado = value;
                OnPropertyChanged();
            }
        }

        public ICommand CerrarTabCommand { get; }
        //termna prueba de ventanas
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

            // ✅ Cerrar pestaña
            CerrarTabCommand = new RelayCommand(() =>
            {
                if (TabSeleccionado != null)
                    Tabs.Remove(TabSeleccionado);
            });

            PlanCuentasCommand = new RelayCommand(() =>
            {
                if (Session.CurrentEmpresa == null) return;

                var existente = Tabs.FirstOrDefault(t => t.Titulo == "Plan de Cuentas");

                if (existente != null)
                {
                    TabSeleccionado = existente;
                    return;
                }

                var nuevaTab = new TabItemViewModel
                {
                    Titulo = "Plan de Cuentas",
                    Contenido = new PlanCuentasViewModel(Session.CurrentEmpresa.IdEmpresa)
                };

                Tabs.Add(nuevaTab);
                TabSeleccionado = nuevaTab;
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