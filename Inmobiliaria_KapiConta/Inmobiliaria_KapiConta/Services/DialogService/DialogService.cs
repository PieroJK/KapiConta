using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models.Dialog;
using Inmobiliaria_KapiConta.ViewModels;
using Inmobiliaria_KapiConta.Views.CustomDialog;


namespace Inmobiliaria_KapiConta.Services.DialogService
{
    public class DialogService : IDialogService
    {
        public void ShowDialog (string name)
        {
            var dialog = new CustomDialog();

            dialog.ShowDialog();
        }
        
    }
}
