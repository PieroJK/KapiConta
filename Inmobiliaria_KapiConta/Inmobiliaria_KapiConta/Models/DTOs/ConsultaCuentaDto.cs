using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class ConsultaCuentaDto
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Periodo { get; set; }
        public decimal Acumulado { get; set; }
    }
}
