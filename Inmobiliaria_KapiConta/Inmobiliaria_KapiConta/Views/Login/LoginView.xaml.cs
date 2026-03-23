using System.Windows;
using System.Windows.Controls;
using Inmobiliaria_KapiConta.ViewModels;

namespace Inmobiliaria_KapiConta.Views.Login
{
    public partial class LoginView : UserControl
    {
        private bool isPasswordVisible = false;

        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)DataContext;
            vm.Password = PasswordBox.Password;
            vm.LoginCommand.Execute(null);
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (isPasswordVisible)
            {
                // Ocultar password
                PasswordBox.Password = VisiblePassword.Text;
                PasswordBox.Visibility = Visibility.Visible;
                VisiblePassword.Visibility = Visibility.Collapsed;

                EyeIcon.Text = "\uE722"; // ??
                isPasswordVisible = false;
            }
            else
            {
                // Mostrar password
                VisiblePassword.Text = PasswordBox.Password;
                VisiblePassword.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Collapsed;

                EyeIcon.Text = "\uE8F4"; // ??
                isPasswordVisible = true;
            }
        }
    }
}