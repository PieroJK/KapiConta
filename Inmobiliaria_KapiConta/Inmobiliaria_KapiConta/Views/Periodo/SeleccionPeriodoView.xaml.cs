using Inmobiliaria_KapiConta.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.Views.Periodo
{
    public partial class SeleccionPeriodoView : UserControl
    {
        public SeleccionPeriodoView()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SeleccionPeriodoViewModel vm)
            {
                if (vm.PeriodoSeleccionado != null)
                {
                    // 🔥 reutilizamos la misma lógica
                    vm.ContinuarCommand.Execute(null);
                }
            }
        }
    }
}
