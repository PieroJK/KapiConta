using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services.UserService;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ListUsersViewModel :INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        
        private List<Usuario> _items;
        private Usuario _selectedUser;
        
        public List<Usuario> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }
        
        public Usuario SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                if (value != null)
                {
                    GetSelectItemData();
                }
            }
        }

        public ICommand VolverCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }

        private void GetSelectItemData()
        {
            // Validación temprana
            if (SelectedUser == null)
            {
                System.Diagnostics.Debug.WriteLine("[ERROR] SelectedUser es null");
                return;
            }
            try
            {
                // Extracción segura de datos
                string idUsuario = SelectedUser.Id.ToString() ?? "0";
                string username = SelectedUser.Username ?? string.Empty;
                string nombre = SelectedUser.Nombre ?? string.Empty;
                string rol = SelectedUser.Rol?.Nombre ?? "Sin rol";
                string estado = SelectedUser.Estado == true ? "Activo" : "Inactivo";

                // Debug completo
                System.Diagnostics.Debug.WriteLine(
                    $"[GetSelectItemData] " +
                    $"ID: {idUsuario} | " +
                    $"User: {username} | " +
                    $"Nombre: {nombre} | " +
                    $"Rol: {rol} | " +
                    $"Estado: {estado}"
                );

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] GetSelectItemData: {ex.Message}");
            }
        }

        private void ListUsers ()
        {
            try
            {
                Items = _userService.ListUser();
                Debug.WriteLine($"Cantidad de la lista: {Items.Count()}");
                Debug.WriteLine($"Tipo de la variable: {Items.GetType}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ALERTA: Excepción");
                Debug.WriteLine(ex.ToString());
            }
            
        }

        private void EditUser ()
        {
            _userService.UpdateUser(SelectedUser);
        }
        
        public ListUsersViewModel(IUserService userService) 

        {
            _userService = userService;
            ModificarCommand = new RelayCommand(EditUser);
            //EliminarCommand = new RelayCommand(Eliminar);
            ListUsers();
        }
    }
}
