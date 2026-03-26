using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.ViewModels;
using System.Windows.Controls;

namespace Inmobiliaria_KapiConta.Views.PlanCuentas
{
    public partial class PlanCuentasView : UserControl
    {
        public PlanCuentasView()
        {
            InitializeComponent();

            if (Session.CurrentEmpresa != null)
            {
                DataContext = new PlanCuentasViewModel(Session.CurrentEmpresa.IdEmpresa);
            }
        }
    }
}
