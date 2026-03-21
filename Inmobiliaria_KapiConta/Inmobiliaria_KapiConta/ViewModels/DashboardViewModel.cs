using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
      

        private string _usuarioActual = "Usuario";
        public string UsuarioActual
        {
            get => _usuarioActual;
            set { _usuarioActual = value; OnPropertyChanged(); }
        }

        private string _empresaActual = "Mi Empresa S.A.C.";
        public string EmpresaActual
        {
            get => _empresaActual;
            set { _empresaActual = value; OnPropertyChanged(); }
        }

        private int _totalEmpresas = 3;
        public int TotalEmpresas
        {
            get => _totalEmpresas;
            set { _totalEmpresas = value; OnPropertyChanged(); }
        }

        private int _totalUsuarios = 12;
        public int TotalUsuarios
        {
            get => _totalUsuarios;
            set { _totalUsuarios = value; OnPropertyChanged(); }
        }

        private int _totalRegistros = 248;
        public int TotalRegistros
        {
            get => _totalRegistros;
            set { _totalRegistros = value; OnPropertyChanged(); }
        }

        private int _procesosPendientes = 5;
        public int ProcesosPendientes
        {
            get => _procesosPendientes;
            set { _procesosPendientes = value; OnPropertyChanged(); }
        }

        public string FechaActual => DateTime.Now.ToString("dd 'de' MMMM, yyyy",
            new System.Globalization.CultureInfo("es-PE"));

        // ══════════════════════════════════════════
        // COMANDOS — EMPRESAS
        // ══════════════════════════════════════════

        public ICommand RegistrarEmpresaCommand { get; }
        public ICommand ListadoEmpresaCommand { get; }
        public ICommand IngresarEmpresaCommand { get; }

        // ══════════════════════════════════════════
        // COMANDOS — USUARIOS
        // ══════════════════════════════════════════

        public ICommand RegistrarUsuarioCommand { get; }
        public ICommand ListadoUsuarioCommand { get; }

        // ══════════════════════════════════════════
        // COMANDOS — SESIÓN
        // ══════════════════════════════════════════

        public ICommand CerrarSesionCommand { get; }

        // ══════════════════════════════════════════
        // EVENTO: notificar al exterior para navegar
        // ══════════════════════════════════════════

        /// <summary>
        /// Se dispara cuando el usuario cierra sesión.
        /// MainWindow escucha este evento para volver al LoginView.
        /// </summary>
        public event Action? SesionCerrada;

        /// <summary>
        /// Se dispara cuando se pulsa un botón de navegación.
        /// Pasa el nombre de la vista destino como string.
        /// </summary>
        public event Action<string>? NavegacionSolicitada;

        // ══════════════════════════════════════════
        // CONSTRUCTOR
        // ══════════════════════════════════════════

        public DashboardViewModel()
        {
            RegistrarEmpresaCommand = new RelayCommand(_ => Navegar("RegistrarEmpresa"));
            ListadoEmpresaCommand = new RelayCommand(_ => Navegar("ListadoEmpresa"));
            IngresarEmpresaCommand = new RelayCommand(_ => Navegar("IngresarEmpresa"));
            RegistrarUsuarioCommand = new RelayCommand(_ => Navegar("RegistrarUsuario"));
            ListadoUsuarioCommand = new RelayCommand(_ => Navegar("ListadoUsuario"));
            CerrarSesionCommand = new RelayCommand(_ => CerrarSesion());
        }

        // ══════════════════════════════════════════
        // MÉTODOS PRIVADOS
        // ══════════════════════════════════════════

        private void Navegar(string destino)
        {
            NavegacionSolicitada?.Invoke(destino);
        }

        private void CerrarSesion()
        {
            var result = MessageBox.Show(
                "¿Estás seguro de que deseas cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                SesionCerrada?.Invoke();
        }

        // ══════════════════════════════════════════
        // INotifyPropertyChanged
        // ══════════════════════════════════════════

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // ══════════════════════════════════════════
    // HELPER: RelayCommand
    // (si ya tienes uno en el proyecto, borra este)
    // ══════════════════════════════════════════

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
