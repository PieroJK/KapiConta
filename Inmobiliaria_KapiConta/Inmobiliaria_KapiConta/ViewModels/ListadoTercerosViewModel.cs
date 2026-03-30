using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ListadoTercerosViewModel : INotifyPropertyChanged
    {
        private readonly TerceroService _terceroService = new TerceroService();

        public ObservableCollection<Tercero> Terceros { get; set; } = new();

        private Tercero _terceroSeleccionado;
        public Tercero TerceroSeleccionado
        {
            get => _terceroSeleccionado;
            set
            {
                _terceroSeleccionado = value;
                OnPropertyChanged();
            }
        }

        private string _textoBuscar;
        public string TextoBuscar
        {
            get => _textoBuscar;
            set
            {
                _textoBuscar = value;
                OnPropertyChanged();
                Buscar();
            }
        }

        private string _filtro = "Razón Social";
        public string Filtro
        {
            get => _filtro;
            set
            {
                _filtro = value;
                OnPropertyChanged();
                Buscar();
            }
        }

        // Overlay
        private object _vistaActual;
        public object VistaActual
        {
            get => _vistaActual;
            set
            {
                _vistaActual = value;
                OnPropertyChanged();
            }
        }

        private Visibility _overlayVisible = Visibility.Collapsed;
        public Visibility OverlayVisible
        {
            get => _overlayVisible;
            set
            {
                _overlayVisible = value;
                OnPropertyChanged();
            }
        }

        // Commands
        public ICommand IngresarCommand { get; }
        public ICommand ExportarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand DobleClickCommand { get; }

        public ListadoTercerosViewModel()
        {
            IngresarCommand = new RelayCommand(AbrirNuevo);
            ExportarCommand = new RelayCommand(Exportar);
            VolverCommand = new RelayCommand(Volver);
            DobleClickCommand = new RelayCommand(AbrirEditar);

            CargarListado();
        }

        private void CargarListado()
        {
            Terceros.Clear();

            var lista = _terceroService.Listar();

            foreach (var item in lista)
                Terceros.Add(item);
        }

        private void Buscar()
        {
            Terceros.Clear();

            var lista = _terceroService.Listar(TextoBuscar, Filtro);

            foreach (var item in lista)
                Terceros.Add(item);
        }

        private void AbrirNuevo()
        {
            var vista = new Views.Terceros.RegistrarTerceroView();
            VistaActual = vista;
            OverlayVisible = Visibility.Visible;
        }

        private void AbrirEditar()
        {
            MessageBox.Show("Luego abrir editor");
        }

        private void Exportar()
        {
            MessageBox.Show("La exportación la hacemos después.");
        }

        private void Volver()
        {
            MessageBox.Show("Luego volver");
        }

        public void CerrarOverlay()
        {
            VistaActual = null;
            OverlayVisible = Visibility.Collapsed;
            CargarListado();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
