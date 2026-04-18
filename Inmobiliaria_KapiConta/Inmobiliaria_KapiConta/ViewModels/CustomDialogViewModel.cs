using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Inmobiliaria_KapiConta.Models.Dialog;
using Inmobiliaria_KapiConta.Services.DialogService;

namespace Inmobiliaria_KapiConta.ViewModels
{
    public class CustomDialogViewModel
    {
        private readonly IDialogService _dialogService;
        
        public CustomDialogViewModel(IDialogService dialogService) 
        {
            _dialogService = dialogService;
        }
    }
}
