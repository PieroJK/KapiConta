using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class AsientoDetalleCargadoItem
    {
        public int IdPlanCuenta { get; set; }
        public string CuentaCodigo { get; set; } = "";
        public string CuentaNombre { get; set; } = "";
        public string Moneda { get; set; } = "PEN";
        public decimal Debe { get; set; }
        public decimal Haber { get; set; }
        public int? IdTipoDocumento { get; set; }
        public string TipoDocumentoCodigo { get; set; } = "";
        public string Documento { get; set; } = "";
        public int? IdTercero { get; set; }
        public string Ruc { get; set; } = "";
        public string RazonSocial { get; set; } = "";
        public string Glosa { get; set; } = "";
        public int? IdTipoOperacion { get; set; }
        public string TipoOperacionNombre { get; set; } = "";
        public string AsientoReferencia { get; set; } = "";
    }
}
