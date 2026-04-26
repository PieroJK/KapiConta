using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Inmobiliaria_KapiConta.Models;
using Inmobiliaria_KapiConta.Services;
using Inmobiliaria_KapiConta.Services.DialogService;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class RegisterEnterpriseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IEnterpriseService _enterpriseService;
        private readonly IDialogService _dialogService = new DialogService();

        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        private Empresa _newEnterprise;
        public Empresa NewEnterprise
        {
            get
            {
                Debug.WriteLine($"Get NewEnterprise: {_newEnterprise != null}");
                return _newEnterprise;
            }
            set
            {
                _newEnterprise = value;
                OnPropertyChanged();
                Debug.WriteLine($"Set NewEnterprise: {value != null}");
            }
        }

        private string _mensajeEstado;
        public string MensajeEstado
        {
            get => _mensajeEstado;
            set { _mensajeEstado = value; OnPropertyChanged(); }
        }

        //IComands para los botones
        public ICommand BtnBuscarCommand { get; }
        public ICommand VolverCommand { get; }
        public ICommand RegistrarCommand { get; }
        
        

        //Constructor
        public RegisterEnterpriseViewModel(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
            //_dialogService = dialogService;
            NewEnterprise = new Empresa();

            RegistrarCommand = new RelayCommand(RegistrarEmpresa);
        }

        //Métodos
        private void RegistrarEmpresa()
        {
            try
            {
                _enterpriseService.AddEnterprise(NewEnterprise);
                Debug.WriteLine("Empresa Registrada");
                //_dialogService.ShowDialog("Notificacion");
                NewEnterprise = new Empresa(); // Limpiando el formulario
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Empresa No Registrada: {ex.Message}");
            }


        }
    }
}
