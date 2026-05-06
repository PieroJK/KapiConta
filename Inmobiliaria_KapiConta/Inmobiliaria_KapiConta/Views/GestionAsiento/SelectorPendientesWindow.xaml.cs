using Inmobiliaria_KapiConta.Models.DTOs;
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
    /// Lógica de interacción para SelectorPendientesWindow.xaml
    /// </summary>
    public partial class SelectorPendientesWindow : Window
    {
        public List<PendienteItem> Resultados =>
            (DataContext as SelectorPendienteViewModel)?.Resultados ?? new();

        public SelectorPendientesWindow(List<PendienteItem> pendientes)
        {
            InitializeComponent();

            var vm = new SelectorPendienteViewModel(pendientes);

            vm.CerrarVentana = resultado =>
            {
                DialogResult = resultado;
                Close();
            };

            DataContext = vm;
        }
    }
}
