using System.Windows;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class LoginViewModel
    {
        public string Usuario { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            var authService = new AuthService();
            Usuario user = authService.Login(Usuario, Password);

            if (user != null)
            {
                //  GUARDAR SESIÓN
                Session.CurrentUser = user;

                MessageBox.Show($"Bienvenido {user.Nombre} ({user.Rol.Nombre}) ?");
            }
            else
            {
                MessageBox.Show("Credenciales incorrectas ?");
            }
        }
    }
}