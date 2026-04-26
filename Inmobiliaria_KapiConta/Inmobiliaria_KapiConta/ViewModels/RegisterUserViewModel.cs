using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services.RolService;
using Inmobiliaria_KapiConta.Services.UserService;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class RegisterUserViewModel:INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IRolService _rolService;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        public ICommand RegistrarCommand => new RelayCommand(RegisterUser);
        //public ICommand RegistrarCommand => new RelayCommand(RegisterUser, CanRegisterUser);
        private string _plainTextPassword;
        public string PlainTextPassword
        {
            get => _plainTextPassword;
            set
            {
                _plainTextPassword = value;
                OnPropertyChanged();
                // Re-evaluar si el comando puede ejecutarse
                (RegistrarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                (RegistrarCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        private List<RolUsuario> _roles;
        public List<RolUsuario> Roles
        {
            get => _roles;
            set { _roles = value; OnPropertyChanged(); }
        }
        private RolUsuario _selectedRol;
        public RolUsuario SelectedRol
        {
            get => _selectedRol;
            set
            {
                _selectedRol = value;
                if (_selectedRol != null)
                {
                    // Asignar el objeto Rol completo
                    NewUser.Rol = _selectedRol;
                }
                OnPropertyChanged();
                Debug.WriteLine($"Rol seleccionado: {SelectedRol?.Nombre}");
                Debug.WriteLine($"ID: {SelectedRol?.IdRol}");
            }
        }

        private Usuario _newUser;
        public Usuario NewUser 
        { 
            get 
            {
                return _newUser;
            }
            set
            {
                _newUser = value;
                OnPropertyChanged();
            } 
        }
        private void RegisterUser()
        {
            try
            {
                Debug.WriteLine($"USERNAME: {NewUser.Username}");
                Debug.WriteLine($"NAME: {NewUser.Nombre}");
                Debug.WriteLine($"ROL: {NewUser.Rol}");
                _userService.AddUser(NewUser,PlainTextPassword);
                Debug.WriteLine("Usuario registrado exitosamente");
                NewUser = new Usuario(); // Limpiando el formulario
                PlainTextPassword = string.Empty;
                ConfirmPassword = string.Empty;
                SelectedRol = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Usuario No Registrado: {ex.Message}");
            }
        }
        private bool CanRegisterUser()
        {
            return NewUser != null &&
                   !string.IsNullOrWhiteSpace(NewUser.Username) &&
                   !string.IsNullOrWhiteSpace(NewUser.Nombre) &&
                   !string.IsNullOrWhiteSpace(PlainTextPassword) &&
                   PlainTextPassword == ConfirmPassword &&
                   SelectedRol != null;
        }
        public RegisterUserViewModel(IUserService userService, IRolService rolService)
        {
            _userService = userService; 
            _rolService = rolService;
            _newUser = new Usuario();
            Roles = rolService.LoadRoles();
           /* foreach (var item in Roles)
            {
                Debug.WriteLine($"[VISTA] ID Rol : {item.IdRol}");
                Debug.WriteLine($"[VISTA] Nombre Rol: {item.Nombre}");
            }*/
            //RegistrarCommand = new RelayCommand(RegisterUser);
            
        }
    }
}
