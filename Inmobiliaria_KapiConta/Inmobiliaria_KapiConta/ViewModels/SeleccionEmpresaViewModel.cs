using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Interfaces;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class SeleccionEmpresaViewModel : INotifyPropertyChanged, IResizableView
    {
        private readonly MainViewModel _mainVM;
        private readonly EmpresaService _empresaService;

        public SeleccionEmpresaViewModel(MainViewModel mainVM)
        {
            _mainVM = mainVM;
            _empresaService = new EmpresaService();

            ContinuarCommand = new RelayCommand(Continuar);

            CargarEmpresas();
        }

        public double Width => 900;
        public double Height => 600;

        // 🔹 Lista para el ComboBox
        public ObservableCollection<Empresa> Empresas { get; set; } = new();

        private Empresa _empresaSeleccionada;
        public Empresa EmpresaSeleccionada
        {
            get => _empresaSeleccionada;
            set
            {
                _empresaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public ICommand ContinuarCommand { get; }

        private void CargarEmpresas()
        {
            var lista = _empresaService.ObtenerEmpresas();

            Empresas.Clear();
            foreach (var emp in lista)
                Empresas.Add(emp);

            if (Empresas.Count > 0)
                EmpresaSeleccionada = Empresas[0];
        }

        private void Continuar()
        {
            if (EmpresaSeleccionada == null)
                return;

            //  Guardar en sesión (BIEN HECHO)
            Session.CurrentEmpresa = EmpresaSeleccionada;

            //  siguiente paso (periodo)
            _mainVM.Navigation.NavigateTo(new SeleccionPeriodoViewModel(_mainVM));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
