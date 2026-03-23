using Inmobiliaria_KapiConta.Views.Enterprise;
using Inmobiliaria_KapiConta.Views.Login;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            var loginVM = new LoginViewModel(this);

            CurrentView = new LoginView
            {
                DataContext = loginVM
            };
        }

        //  Método para cambiar a selección de empresas
        public void CambiarASeleccionEmpresa()
        {
            CurrentView = new EnterpriseSelectionView();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}