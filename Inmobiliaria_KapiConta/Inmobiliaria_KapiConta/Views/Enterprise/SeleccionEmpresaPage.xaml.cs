using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Inmobiliaria_KapiConta.ViewModels;

namespace Inmobiliaria_KapiConta.Views.Enterprise
{
    public partial class SeleccionEmpresaPage : UserControl
    {
        public SeleccionEmpresaPage()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SeleccionEmpresaViewModel vm)
            {
                if (vm.EmpresaSeleccionada != null)
                {
                    // 🔥 reutilizamos la misma lógica
                    vm.ContinuarCommand.Execute(null);
                }
            }
        }
    }
}