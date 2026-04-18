using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using Inmobiliaria_KapiConta.Views.Enterprise;
using Inmobiliaria_KapiConta.Services;

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
        private readonly EnterpriseService service;
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

        public ICommand RegisterEnterpriseCommand { get; }
        public ICommand ListEnterpriseCommand { get; }
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
            CerrarTabCommand = new RelayCommand<object>((param) =>
            {
                if (TabSeleccionado != null)
                    Tabs.Remove(TabSeleccionado);
            });

            // Generación de UserControl para registrar empresa en el dashboard
            RegisterEnterpriseCommand = new RelayCommand(() =>
            {
                if (Session.CurrentEmpresa == null) return;
                var existente = Tabs.FirstOrDefault(t => t.Titulo == "Registrar empresa");

                if (existente != null)
                {
                    TabSeleccionado = existente;
                    return;
                }

                var service = new EnterpriseService();
                var vm = new RegisterEnterpriseViewModel(service);
                var nuevaTab = new TabItemViewModel
                {
                    Titulo = "Registrar empresa",
                    Contenido = vm
                };

                Tabs.Add(nuevaTab);
                TabSeleccionado = nuevaTab;
            });

            ListEnterpriseCommand = new RelayCommand(() =>
            {
                if (Session.CurrentEmpresa == null) return;
                var existente = Tabs.FirstOrDefault(t => t.Titulo == "Listar empresa");

                if (existente != null)
                {
                    TabSeleccionado = existente;
                    return;
                }
                var service = new EnterpriseService();
                var vm = new ListEnterpriseViewModel(service);
                var nuevaTab = new TabItemViewModel
                {
                    Titulo = "Listar empresa",
                    Contenido = vm
                };
                Tabs.Add(nuevaTab);
                TabSeleccionado = nuevaTab;
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
            {
                try
                {
                    if (Session.CurrentEmpresa == null) return;

                    var existente = Tabs.FirstOrDefault(t => t.Titulo == "Registrar Terceros");

                    if (existente != null)
                    {
                        TabSeleccionado = existente;
                        return;
                    }

                    var vm = new TerceroViewModel(); // 🔥 aquí probablemente rompe

                    var nuevaTab = new TabItemViewModel
                    {
                        Titulo = "Registrar Terceros",
                        Contenido = vm
                    };

                    Tabs.Add(nuevaTab);
                    TabSeleccionado = nuevaTab;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {DateTime.Now}");
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            });

            ListadoTercerosCommand = new RelayCommand(() =>
            {
                try
                {
                    if (Session.CurrentEmpresa == null) return;

                    var existente = Tabs.FirstOrDefault(t => t.Titulo == "Listado Terceros");

                    if (existente != null)
                    {
                        TabSeleccionado = existente;
                        return;
                    }

                    var vm = new ListadoTercerosViewModel(); // 🔥 aquí probablemente rompe

                    vm.AbrirRegistrarTercero = () =>
                    {
                        RegistrarTercerosCommand.Execute(null);
                    };

                    var nuevaTab = new TabItemViewModel
                    {
                        Titulo = "Listado Terceros",
                        Contenido = vm
                    };

                    Tabs.Add(nuevaTab);
                    TabSeleccionado = nuevaTab;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ERROR] {DateTime.Now}");
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            });

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}