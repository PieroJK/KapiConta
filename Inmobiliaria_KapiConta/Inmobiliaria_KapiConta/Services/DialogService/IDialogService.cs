using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inmobiliaria_KapiConta.Helpers;
using Inmobiliaria_KapiConta.Models.Dialog;

namespace Inmobiliaria_KapiConta.Services.DialogService
{
    public interface IDialogService
    {
        void ShowDialog(string name);
    }
}