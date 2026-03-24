using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Interfaces; // 🔥 IMPORTANTE

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class EnterpriseSelectionViewModel : INotifyPropertyChanged, IResizableView
    {
        private readonly MainViewModel _mainVM;

        public EnterpriseSelectionViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;

            // Datos de prueba (luego BD)
            Empresas = new ObservableCollection<string>
            {
                "Empresa 1",
                "Empresa 2",
                "Empresa 3"
            };

            SeleccionarEmpresaCommand = new RelayCommand(SeleccionarEmpresa);
        }

        // 🔥 TAMAÑO AUTOMÁTICO
        public double Width => 362;
        public double Height => 500;

        public ObservableCollection<string> Empresas { get; set; }

        private string _empresaSeleccionada;
        public string EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set
            {
                _empresaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public ICommand SeleccionarEmpresaCommand { get; }

        private void SeleccionarEmpresa()
        {
            if (EmpresaSeleccionada == null)
                return;

            // 🔥 AQUÍ YA NO USAS CambiarVista
            // luego puedes hacer:
            // _mainVM.Navigation.NavigateTo(new DashboardViewModel(_mainVM));

            System.Windows.MessageBox.Show($"Empresa seleccionada: {EmpresaSeleccionada}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}