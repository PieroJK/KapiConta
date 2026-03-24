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

        // 🔹 Tamaño de ventana
        private double _windowWidth;
        public double WindowWidth
        {
            get => _windowWidth;
            set { _windowWidth = value; OnPropertyChanged(); }
        }

        private double _windowHeight;
        public double WindowHeight
        {
            get => _windowHeight;
            set { _windowHeight = value; OnPropertyChanged(); }
        }

        // 🔥 Constructor (pantalla inicial)
        public MainViewModel()
        {
            CurrentView = new LoginView(this);
            WindowWidth = 950;
            WindowHeight = 600;
        }

        // 🔹 Navegación a selección de empresa
        public void CambiarASeleccionEmpresa()
        {
            CurrentView = new EnterpriseSelectionView(this);

            WindowWidth = 362;
            WindowHeight = 500;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}