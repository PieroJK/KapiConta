using Inmobiliaria_KapiConta.Interfaces;
using Inmobiliaria_KapiConta.ViewModels;
using System.Windows;

namespace Inmobiliaria_KapiConta.Services
{
    public class NavigationService
    {
        private readonly Action<object> _setView;
        private readonly MainViewModel _main;

        public NavigationService(Action<object> setView, MainViewModel main)
        {
            _setView = setView;
            _main = main;
        }

        public void NavigateTo(object viewModel)
        {
            _setView(viewModel);

            if (viewModel is IResizableView v)
            {
                _main.WindowWidth = v.Width;
                _main.WindowHeight = v.Height;

                // 🔥 RECENTRAR VENTANA
                Application.Current.MainWindow.Left =
                    (SystemParameters.PrimaryScreenWidth - v.Width) / 2;

                Application.Current.MainWindow.Top =
                    (SystemParameters.PrimaryScreenHeight - v.Height) / 2;
            }
        }
    }
}
