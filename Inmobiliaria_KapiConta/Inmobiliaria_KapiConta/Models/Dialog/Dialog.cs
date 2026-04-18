using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Models.Dialog
{
    public class Dialog
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DialogType Type { get; set; }
        public string Details { get; set; }  // Para errores técnicos
        public bool ShowConfirmButtons { get; set; }  // Sí/No vs Solo OK
    }
}
