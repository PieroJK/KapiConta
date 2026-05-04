using Inmobiliaria_KapiConta.ViewModels;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Inmobiliaria_KapiConta.Views.GestionAsiento
{
    /// <summary>
    /// Lógica de interacción para AgregarDetalleAsientoWindow.xaml
    /// </summary>
    public partial class AgregarDetalleAsientoWindow : Window
    {
        public AgregarDetalleAsientoWindow()
        {
            InitializeComponent();
        }

        // Agrega estos dos métodos al code-behind
        private void txtCuentaCodigo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is AgregarDetalleAsientoViewModel vm)
                vm.CuentaCodigoLostFocusCommand.Execute(null);
        }

        private void txtRuc_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is AgregarDetalleAsientoViewModel vm)
                vm.RucLostFocusCommand.Execute(null);
        }
    }
}
