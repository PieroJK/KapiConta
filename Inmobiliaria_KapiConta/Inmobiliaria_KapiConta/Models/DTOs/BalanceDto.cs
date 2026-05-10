using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class BalanceDto
    {
        public string Codigo { get; set; }
        public string Cuenta { get; set; }
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public decimal Deudor { get; set; }
        public decimal Acreedor { get; set; }
        public decimal Activo { get; set; }
        public decimal Pasivo { get; set; }
        public decimal ResultadoDebe { get; set; }
        public decimal ResultadoHaber { get; set; }
    }
}
