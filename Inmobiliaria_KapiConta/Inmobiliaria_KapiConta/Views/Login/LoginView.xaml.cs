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
    }
}