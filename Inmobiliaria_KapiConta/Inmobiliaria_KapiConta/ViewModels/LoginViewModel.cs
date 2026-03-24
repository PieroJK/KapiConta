using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainVM;

        public LoginViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            LoginCommand = new RelayCommand(Login);
        }

        private string _usuario;
        public string Usuario
        {
            get => _usuario;
            set
            {
                _usuario = value;
                OnPropertyChanged();
                ValidarUsuario();
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                ValidarPassword();
            }
        }

        private string _errorUsuario;
        public string ErrorUsuario
        {
            get => _errorUsuario;
            set { _errorUsuario = value; OnPropertyChanged(); }
        }

        private string _errorPassword;
        public string ErrorPassword
        {
            get => _errorPassword;
            set { _errorPassword = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }


        private void ValidarUsuario()
        {
            if (string.IsNullOrEmpty(Usuario) || Usuario.Length <= 3)
                ErrorUsuario = "Debe tener más de 3 caracteres";
            else
                ErrorUsuario = "";
        }

        private void ValidarPassword()
        {
            if (string.IsNullOrEmpty(Password) || Password.Length <= 5)
                ErrorPassword = "Debe tener más de 5 caracteres";
            else
                ErrorPassword = "";
        }

        private void Login()
        {
            // ?? Validar antes de consultar BD
            ValidarUsuario();
            ValidarPassword();

            if (!string.IsNullOrEmpty(ErrorUsuario) || !string.IsNullOrEmpty(ErrorPassword))
                return;

            var authService = new AuthService();
            Usuario user = authService.Login(Usuario, Password);

            if (user != null)
            {
                Session.CurrentUser = user;
             
                ErrorPassword = "";
                ErrorUsuario = "";

                //  CAMBIO DE VISTA (MVVM PURO)
                _mainVM.CambiarASeleccionEmpresa();
            }
            else
            {
                ErrorPassword = "Credenciales incorrectas";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}