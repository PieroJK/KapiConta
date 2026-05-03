using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class SelectorTerceroViewModel : INotifyPropertyChanged
    {
        private readonly TerceroService _service;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // BÚSQUEDA
        // =========================

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
                CollectionViewSource.GetDefaultView(Terceros)?.Refresh();
            }
        }

        // =========================
        // DATOS
        // =========================

        private ObservableCollection<Tercero> _terceros;
        public ObservableCollection<Tercero> Terceros
        {
            get => _terceros;
            set { _terceros = value; OnPropertyChanged(); }
        }

        private Tercero _terceroSeleccionado;
        public Tercero TerceroSeleccionado
        {
            get => _terceroSeleccionado;
            set { _terceroSeleccionado = value; OnPropertyChanged(); }
        }

        // =========================
        // COMMANDS
        // =========================

        public ICommand SeleccionarCommand { get; }

        // =========================
        // RESULTADO
        // =========================

        public Action<Tercero> OnTerceroSeleccionado { get; set; }

        // =========================
        // CONSTRUCTOR
        // =========================

        public SelectorTerceroViewModel()
        {
            _service = new TerceroService();

            SeleccionarCommand = new RelayCommand(Seleccionar);

            CargarTerceros();
        }

        // =========================
        // CARGA
        // =========================

        private void CargarTerceros()
        {
            var lista = _service.Listar();
            Terceros = new ObservableCollection<Tercero>(lista);
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var view = CollectionViewSource.GetDefaultView(Terceros);
            view.Filter = item =>
            {
                if (string.IsNullOrWhiteSpace(TextoBusqueda)) return true;
                var tercero = item as Tercero;
                return tercero.Documento.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase)
                    || tercero.RazonSocial.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase);
            };
        }

        // =========================
        // SELECCIONAR (pendiente)
        // =========================

        private void Seleccionar()
        {
            if (TerceroSeleccionado == null) return;

            OnTerceroSeleccionado?.Invoke(TerceroSeleccionado);
        }
    }
}