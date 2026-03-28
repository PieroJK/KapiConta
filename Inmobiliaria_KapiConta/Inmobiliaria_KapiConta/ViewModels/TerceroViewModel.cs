using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class TerceroViewModel : INotifyPropertyChanged
    {
        private readonly TerceroService _service = new();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // PROPIEDADES
        // =========================

        public ObservableCollection<TerceroTipoDocumento> TiposDocumento { get; set; } = new();

        private TerceroTipoDocumento _tipoSeleccionado;
        public TerceroTipoDocumento TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();
                ActualizarEtiquetaDocumento();
                NombreTipoDocumento = value?.Nombre ?? "";
            }
        }

        private string _documento;
        public string Documento
        {
            get => _documento;
            set
            {
                _documento = value;
                ValidarLongitudDocumento();
                OnPropertyChanged();
            }
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

        private string _condicion = "HABIDO";
        public string Condicion
        {
            get => _condicion;
            set { _condicion = value; OnPropertyChanged(); }
        }

        public string Estado => "Activo";

        private string _nombreTipoDocumento;
        public string NombreTipoDocumento
        {
            get => _nombreTipoDocumento;
            set { _nombreTipoDocumento = value; OnPropertyChanged(); }
        }

        private string _labelDocumento = "Documento";
        public string LabelDocumento
        {
            get => _labelDocumento;
            set { _labelDocumento = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand RegistrarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand SunatCommand { get; }
        public ICommand DniCommand { get; }
        public ICommand ImportarCommand { get; }

        public event Action CerrarRequested;

        // =========================
        // CONSTRUCTOR
        // =========================

        public TerceroViewModel()
        {
            RegistrarCommand = new RelayCommand(Registrar);
            VolverCommand = new RelayCommand(() => CerrarRequested?.Invoke());
            SunatCommand = new RelayCommand(AbrirSunat);
            DniCommand = new RelayCommand(AbrirDni);
            ImportarCommand = new RelayCommand(() =>
                MessageBox.Show("La importación desde Excel la dejamos para el siguiente paso."));

            CargarTiposDocumento();
        }

        // =========================
        // MÉTODOS
        // =========================

        private void CargarTiposDocumento()
        {
            TiposDocumento = new ObservableCollection<TerceroTipoDocumento>(
                _service.ObtenerTiposDocumento()
            );

            OnPropertyChanged(nameof(TiposDocumento));
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

        private void ValidarLongitudDocumento()
        {
            if (TipoSeleccionado == null || string.IsNullOrEmpty(Documento))
                return;

            if (TipoSeleccionado.Cod == 6 && Documento.Length > 11)
                Documento = Documento.Substring(0, 11);

            if (TipoSeleccionado.Cod == 1 && Documento.Length > 8)
                Documento = Documento.Substring(0, 8);
        }

        private void Registrar()
        {
            try
            {
                if (TipoSeleccionado == null)
                {
                    MessageBox.Show("Seleccione el tipo de documento.");
                    return;
                }

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

                if (string.IsNullOrWhiteSpace(RazonSocial))
                {
                    MessageBox.Show("Ingrese la razón social.");
                    return;
                }

                if (_service.ExisteDocumento(Documento))
                {
                    MessageBox.Show("El documento ya está registrado.");
                    return;
                }

                var tercero = new Tercero
                {
                    Documento = Documento,
                    RazonSocial = RazonSocial,
                    Direccion = Direccion,
                    Estado = true,
                    Condicion = Condicion,
                    IdTerceroTipoDocumento = TipoSeleccionado.IdTerceroTipoDocumento
                };

                _service.Insertar(tercero);

                MessageBox.Show("Tercero registrado correctamente.");
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Limpiar()
        {
            Documento = "";
            RazonSocial = "";
            Direccion = "";
            Condicion = "HABIDO";
            TipoSeleccionado = null;
            NombreTipoDocumento = "";
            LabelDocumento = "Documento";
        }

        private void AbrirSunat()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://e-consultaruc.sunat.gob.pe",
                UseShellExecute = true
            });
        }

        private void AbrirDni()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://eldni.com/pe/buscar-datos-por-dni",
                UseShellExecute = true
            });
        }
    }
}