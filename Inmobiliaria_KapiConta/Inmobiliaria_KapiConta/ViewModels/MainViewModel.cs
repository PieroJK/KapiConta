using Inmobiliaria_KapiConta.Services;
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
        private double _windowWidth = 900;
        public double WindowWidth
        {
            get => _windowWidth;
            set
            {
                _windowWidth = value;
                OnPropertyChanged();
            }
        }

        private double _windowHeight = 600;
        public double WindowHeight
        {
            get => _windowHeight;
            set
            {
                _windowHeight = value;
                OnPropertyChanged();
            }
        }

        // 🔥 Navigation Service
        public NavigationService Navigation { get; }

        public MainViewModel()
        {
            Navigation = new NavigationService(viewModel =>
            {
                CurrentView = viewModel;
            }, this);

            // Vista inicial
            Navigation.NavigateTo(new LoginViewModel(this));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}