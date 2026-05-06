using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Models.DTOs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class SelectorPendienteViewModel : INotifyPropertyChanged
    {
        // =========================
        // INFRAESTRUCTURA
        // =========================

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // DEPENDENCIAS EXTERNAS
        // =========================

        private readonly List<PendienteItem> _original;
        public Action<bool> CerrarVentana { get; set; }

        // =========================
        // RESULTADO
        // =========================

        public List<PendienteItem> Resultados { get; private set; } = new();

        // =========================
        // COLECCIONES
        // =========================

        private ObservableCollection<PendienteItem> _pendientes;
        public ObservableCollection<PendienteItem> Pendientes
        {
            get => _pendientes;
            set { _pendientes = value; OnPropertyChanged(); }
        }

        // =========================
        // TEXTOS
        // =========================

        private string _textoBuscar = "";
        public string TextoBuscar
        {
            get => _textoBuscar;
            set
            {
                _textoBuscar = value;
                OnPropertyChanged();
                AplicarFiltro();
            }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand AceptarCommand { get; }
        public ICommand CancelarCommand { get; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public SelectorPendienteViewModel(List<PendienteItem> pendientes)
        {
            _original = pendientes;

            AceptarCommand = new RelayCommand(Aceptar);
            CancelarCommand = new RelayCommand(Cancelar);

            Pendientes = new ObservableCollection<PendienteItem>(_original);
        }

        // =========================
        // MÉTODOS
        // =========================

        private void AplicarFiltro()
        {
            string filtro = (_textoBuscar ?? "").Trim().ToUpper();

            var filtrados = string.IsNullOrEmpty(filtro)
                ? _original
                : _original.Where(x =>
                    x.Cuenta.ToUpper().Contains(filtro) ||
                    x.Documento.ToUpper().Contains(filtro) ||
                    x.Proveedor.ToUpper().Contains(filtro) ||
                    x.Ruc.Contains(filtro)
                ).ToList();

            Pendientes = new ObservableCollection<PendienteItem>(filtrados);
        }

        private void Aceptar()
        {
            Resultados = _original.Where(x => x.IsSelected).ToList();

            if (Resultados.Count == 0)
            {
                MessageBox.Show(
                    "Por favor, selecciona al menos un pendiente para matarlo.",
                    "Aviso",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            CerrarVentana?.Invoke(true);
        }

        private void Cancelar()
        {
            CerrarVentana?.Invoke(false);
        }
    }
}
