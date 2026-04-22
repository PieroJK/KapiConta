using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
        private Empresa _selectedItem;

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

        public Empresa SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged();

                if (value != null)
                {
                    GetSelectItemData();
                }
            }
        }
        
        private void GetSelectItemData()
        {
            string ruc = SelectedItem.Ruc;
            string nombre = SelectedItem.Nombre;
            string estado =  SelectedItem.Estado.ToString();
            string direccion = SelectedItem.Direccion;

            // Debug
            System.Diagnostics.Debug.WriteLine($"Seleccionado: {nombre} - RUC: {ruc}");
        }
        private void ListEnterprise()
        {
            Items = _enterpriseService.ListEnterprise();
        }

        public ListEnterpriseViewModel(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
            ListEnterprise();

            VolverCommand = new RelayCommand(Volver);
            ModificarCommand = new RelayCommand(Modificar);
            EliminarCommand = new RelayCommand(Eliminar);
        }

        private void Volver() { /* Navegar atrás */ }
        private void Modificar() 
        {
            try
            {
                _enterpriseService.UpdateEnterprise(SelectedItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Empresa No Actualizada: {ex.Message}");
            }
            
            
        }
        private void Eliminar() 
        {
            try
            {
                _enterpriseService.DeleteEnterprise(SelectedItem);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Empresa No Eliminada: {ex.Message}");
            }
        }
    }
}
