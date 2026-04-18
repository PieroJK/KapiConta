using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class ListEnterpriseViewModel : INotifyPropertyChanged
    {
        private readonly IEnterpriseService _enterpriseService;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private List<Empresa> _items;
        private string _selectedItem;

        public ICommand VolverCommand { get; }
        public ICommand ModificarCommand { get; }
        public ICommand EliminarCommand { get; }

        public List<Empresa> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
                // Aquí puedes ejecutar lógica cuando se selecciona un item
                SelectItem();
            }
        }
        
        private void SelectItem()
        {
            if (!string.IsNullOrEmpty(SelectedItem))
            {
                // Lógica cuando se selecciona un item
                System.Diagnostics.Debug.WriteLine($"Seleccionado: {SelectedItem}");
            }
        }
        private void ListEnterprise()
        {
            Items = _enterpriseService.ListEnterprise();
            //_enterpriseService.ListEnterprise();
        }

        public ListEnterpriseViewModel(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
            ListEnterprise();

            VolverCommand = new RelayCommand(Volver);
            ModificarCommand = new RelayCommand(Modificar, () => SelectedItem != null);
            EliminarCommand = new RelayCommand(Eliminar, () => SelectedItem != null);
        }

        private void Volver() { /* Navegar atrás */ }
        private void Modificar() { /* Lógica de modificación */ }
        private void Eliminar() { /* Lógica de eliminación */ }
    }
}
