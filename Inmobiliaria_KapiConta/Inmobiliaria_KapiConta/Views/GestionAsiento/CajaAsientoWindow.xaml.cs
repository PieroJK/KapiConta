using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para CajaAsientoWindow.xaml
    /// </summary>
    public partial class CajaAsientoWindow : Window
    {
        public CajaAsientoWindow(ObservableCollection<AsientoDetalle> detalle)
        {
            InitializeComponent();

            var vm = new CajaAsientoViewModel(detalle);

            vm.CerrarVentana = resultado =>
            {
                DialogResult = resultado;
                Close();
            };

            DataContext = vm;
        }
    }
}
