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
    /// Lógica de interacción para SelectorTerceroWindow.xaml
    /// </summary>
    public partial class SelectorTerceroWindow : Window
    {
        public SelectorTerceroWindow()
        {
            InitializeComponent();
        }
        private void DG_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SelectorTerceroViewModel vm)
            {
                vm.SeleccionarCommand.Execute(null);
            }
        }
    }
}
