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
    /// Lógica de interacción para SelectorCuentaWindow.xaml
    /// </summary>
    public partial class SelectorCuentaWindow : Window
    {
        public SelectorCuentaWindow()
        {
            InitializeComponent();
        }
        private void DG_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SelectorCuentaViewModel vm)
            {
                vm.SeleccionarCommand.Execute(null);
            }
        }
    }
}
