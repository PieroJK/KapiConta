using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.ViewModels;

namespace Inmobiliaria_KapiConta.Views.Enterprise
{
    /// <summary>
    /// Interaction logic for RegisterEnterpriseView.xaml
    /// </summary>
    public partial class RegisterEnterpriseView : UserControl
    {
        public RegisterEnterpriseView()
        {
            InitializeComponent();
            var service = new EnterpriseService();
            var vm = new RegisterEnterpriseViewModel(service);
            DataContext = vm;

            Debug.WriteLine($"DataContext: {DataContext}");
            Debug.WriteLine($"NewEnterprise: {vm.NewEnterprise}");
            Debug.WriteLine($"Ruc: {vm.NewEnterprise?.Ruc}");
        }
    }
}
