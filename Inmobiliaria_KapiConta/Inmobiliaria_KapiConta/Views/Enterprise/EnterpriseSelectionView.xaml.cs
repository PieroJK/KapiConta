using System.Windows;
using System.Windows.Controls;
using Inmobiliaria_KapiConta.ViewModels;

namespace Inmobiliaria_KapiConta.Views.Enterprise
{
    public partial class EnterpriseSelectionView : UserControl
    {
        public EnterpriseSelectionView(MainViewModel mainVM)
        {
            InitializeComponent();
            DataContext = new EnterpriseSelectionViewModel(mainVM);
        }
    }
}