using System.Windows;
using System.Windows.Controls;

namespace Inmobiliaria_KapiConta.Views.Login
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)DataContext;
            vm.Password = PasswordBox.Password; // aquí pasas el password
            vm.LoginCommand.Execute(null);
        }
    }
}