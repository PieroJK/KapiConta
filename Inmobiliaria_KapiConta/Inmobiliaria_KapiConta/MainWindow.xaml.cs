using Inmobiliaria_KapiConta.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Inmobiliaria_KapiConta.Helpers;

namespace Inmobiliaria_KapiConta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var vm = new MainViewModel();
            DataContext = vm;

            // Inicial
            this.Loaded += (s, e) =>
            {
                this.Width = vm.WindowWidth;
                this.Height = vm.WindowHeight;

                this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
                this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
            };

            // Cambios dinámicos
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.WindowWidth) || e.PropertyName == nameof(vm.WindowHeight))
                {
                    this.Width = vm.WindowWidth;
                    this.Height = vm.WindowHeight;

                    this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
                    this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
                }
            };
            // 🔐 Generar hash
            //string hash = PasswordHelper.HashPassword("123456");

            // 👇 Mostrar resultado
            //MessageBox.Show(hash);
        }
    }
}