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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inmobiliaria_KapiConta.Views.Terceros
{
    /// <summary>
    /// Lógica de interacción para ListadoTercerosView.xaml
    /// </summary>
    public partial class ListadoTercerosView : UserControl
    {
        public ListadoTercerosView()
        {
            InitializeComponent();
        }

        private void dgTerceros_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is ListadoTercerosViewModel vm)
            {
                if (vm.DobleClickCommand.CanExecute(null))
                    vm.DobleClickCommand.Execute(null);
            }
        }
    }
}
