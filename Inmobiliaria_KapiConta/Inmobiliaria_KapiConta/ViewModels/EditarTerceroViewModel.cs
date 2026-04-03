using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class EditarTerceroViewModel : INotifyPropertyChanged
    {
        private readonly TerceroService _service = new TerceroService();

        public ObservableCollection<TerceroTipoDocumento> TiposDocumento { get; set; } = new();
        public int IdTercero { get; set; }

        public Action RefrescarListado { get; set; }

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

        private int _idTipoDocumento;
        public int IdTerceroTipoDocumento
        {
            get => _idTipoDocumento;
            set { 
                _idTipoDocumento = value;
                ActualizarEtiquetaDocumento();
                OnPropertyChanged(); 
            }
        }

        private bool _estado;
        public bool Estado
        {
            get => _estado;
            set { 
                _estado = value; OnPropertyChanged(); 
            }
        }

        private string _labelDocumento = "Documento";
        public string LabelDocumento
        {
            get => _labelDocumento;
            set { _labelDocumento = value; OnPropertyChanged(); }
        }

        //agregando

        private TerceroTipoDocumento _tipoSeleccionado;
        public TerceroTipoDocumento TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();

                if (value != null)
                {
                    IdTerceroTipoDocumento = value.IdTerceroTipoDocumento;
                    NombreTipoDocumento = value.Nombre;
                }

                ActualizarEtiquetaDocumento();
            }
        }

        private string _nombreTipoDocumento;
        public string NombreTipoDocumento
        {
            get => _nombreTipoDocumento;
            set { _nombreTipoDocumento = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        public ICommand CerrarCommand { get; }

        public Action Cerrar { get; set; }

        public EditarTerceroViewModel(Tercero t)
        {
            CargarTiposDocumento(); // 🔥 IMPORTANTE PRIMERO

            var terceroCompleto = _service.ObtenerPorDocumento(t.Documento);

            if (terceroCompleto != null)
            {
                IdTercero = terceroCompleto.IdTercero;
                Documento = terceroCompleto.Documento;
                RazonSocial = terceroCompleto.RazonSocial;
                Direccion = terceroCompleto.Direccion;
                Condicion = terceroCompleto.Condicion;

                IdTerceroTipoDocumento = terceroCompleto.IdTerceroTipoDocumento;
                Estado = terceroCompleto.Estado;

                // 🔥 AQUÍ ESTÁ LA MAGIA
                TipoSeleccionado = TiposDocumento
                    .FirstOrDefault(x => x.IdTerceroTipoDocumento == IdTerceroTipoDocumento);
            }

            GuardarCommand = new RelayCommand(Guardar);
            CerrarCommand = new RelayCommand(() => Cerrar?.Invoke());
        }

        private void ActualizarEtiquetaDocumento()
        {
            if (TipoSeleccionado == null)
            {
                LabelDocumento = "Documento";
                return;
            }

            LabelDocumento = TipoSeleccionado.Cod switch
            {
                6 => "Ruc",
                1 => "Dni",
                7 => "Pasaporte",
                _ => "Documento"
            };
        }

        private void CargarTiposDocumento()
        {
            TiposDocumento = new ObservableCollection<TerceroTipoDocumento>(
                _service.ObtenerTiposDocumento()
            );

            OnPropertyChanged(nameof(TiposDocumento));
        }
        private void Guardar()
        {
            try
            {
                // 🔥 VALIDAR TIPO DOCUMENTO
                if (TipoSeleccionado == null)
                {
                    MessageBox.Show("Seleccione el tipo de documento.");
                    return;
                }

                // 🔥 VALIDAR DOCUMENTO
                if (string.IsNullOrWhiteSpace(Documento))
                {
                    MessageBox.Show("Ingrese el documento.");
                    return;
                }

                if (TipoSeleccionado.Cod == 6 && Documento.Length != 11)
                {
                    MessageBox.Show("El RUC debe tener 11 dígitos.");
                    return;
                }

                if (TipoSeleccionado.Cod == 1 && Documento.Length != 8)
                {
                    MessageBox.Show("El DNI debe tener 8 dígitos.");
                    return;
                }

                // 🔥 VALIDAR RAZON SOCIAL
                if (string.IsNullOrWhiteSpace(RazonSocial))
                {
                    MessageBox.Show("Ingrese la razón social.");
                    return;
                }

                // 🔥 VALIDAR DUPLICADO (IMPORTANTE EN EDITAR)
                var existente = _service.ObtenerPorDocumento(Documento);
                if (existente != null && existente.IdTercero != IdTercero)
                {
                    MessageBox.Show("El documento ya pertenece a otro registro.");
                    return;
                }

                // 🔥 GUARDAR
                _service.Actualizar(new Tercero
                {
                    IdTercero = IdTercero,
                    Documento = Documento,
                    RazonSocial = RazonSocial,
                    Direccion = Direccion,
                    Condicion = Condicion,
                    Estado = Estado,
                    IdTerceroTipoDocumento = IdTerceroTipoDocumento
                });

                MessageBox.Show("Tercero actualizado correctamente.");
                RefrescarListado?.Invoke();
                Cerrar?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
