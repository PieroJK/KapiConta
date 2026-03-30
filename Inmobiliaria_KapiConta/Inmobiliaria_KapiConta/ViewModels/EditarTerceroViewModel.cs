using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class EditarTerceroViewModel : INotifyPropertyChanged
    {
        private readonly TerceroService _service = new TerceroService();

        public int IdTercero { get; set; }

        private string _documento;
        public string Documento
        {
            get => _documento;
            set { _documento = value; OnPropertyChanged(); }
        }

        private string _razonSocial;
        public string RazonSocial
        {
            get => _razonSocial;
            set { _razonSocial = value; OnPropertyChanged(); }
        }

        private string _direccion;
        public string Direccion
        {
            get => _direccion;
            set { _direccion = value; OnPropertyChanged(); }
        }

        private string _condicion;
        public string Condicion
        {
            get => _condicion;
            set { _condicion = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }

        public Action Cerrar { get; set; }

        public EditarTerceroViewModel(Tercero t)
        {
            IdTercero = t.IdTercero;
            Documento = t.Documento;
            RazonSocial = t.RazonSocial;
            Direccion = t.Direccion;
            Condicion = t.Condicion;

            GuardarCommand = new RelayCommand(Guardar);
            CerrarCommand = new RelayCommand(() => Cerrar?.Invoke());
        }

        private void Guardar()
        {
            _service.Actualizar(new Tercero
            {
                IdTercero = IdTercero,
                Documento = Documento,
                RazonSocial = RazonSocial,
                Direccion = Direccion,
                Condicion = Condicion,
                Estado = true,
                IdTerceroTipoDocumento = 1 // luego lo mejoras
            });

            Cerrar?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
