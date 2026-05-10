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
using Inmobiliaria_KapiConta.ViewModels;

namespace Inmobiliaria_KapiConta.Views.GestionAsiento
{
    /// <summary>
    /// Lógica de interacción para BuscarAsientoWindow.xaml
    /// </summary>
    public partial class BuscarAsientoWindow : Window
    {
        public int? IdAsientoSeleccionado { get; private set; }
        public BuscarAsientoWindow()
        {
            InitializeComponent();

            var vm = new BuscarAsientoViewModel();

            vm.CerrarVentana = resultado =>
            {
                DialogResult = resultado;
                Close();
            };

            vm.AsientoSeleccionado = id =>
            {
                IdAsientoSeleccionado = id;
            };

            DataContext = vm;
        }

        // 🔥 doble clic delega al VM via comando
        private void dgAsientos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is BuscarAsientoViewModel vm)
                vm.SeleccionarCommand.Execute(null);
        }
    }
}
