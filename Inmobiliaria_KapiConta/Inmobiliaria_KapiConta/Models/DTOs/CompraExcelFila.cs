namespace Inmobiliaria_KapiConta.Models.DTOs
{
    //Fila cruda del Excel de Compras
    public class CompraExcelFila
    {
        public DateTime Fecha { get; set; }
        public DateTime? FechaVcmto { get; set; }
        public string TipoDoc { get; set; } = string.Empty;
        public string Serie { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public string Ident { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Costo { get; set; } = string.Empty;
        public string Glosa { get; set; } = string.Empty;
        public decimal Exonerado { get; set; }
        public decimal Inafecto { get; set; }
        public decimal Imponible1 { get; set; }
        public decimal Igv1 { get; set; }
        public decimal Imponible2 { get; set; }
        public decimal Igv2 { get; set; }
        public decimal Imponible3 { get; set; }
        public decimal Igv3 { get; set; }
        public decimal Imponible4 { get; set; }
        public decimal Igv4 { get; set; }
        public decimal Imponible5 { get; set; }
        public decimal Igv5 { get; set; }
        public decimal Total { get; set; }
        public string Moneda { get; set; } = "PEN";
        public decimal TipoCambio { get; set; } = 1;
        public string CtaExonerada { get; set; } = string.Empty;
        public string CtaInafecta { get; set; } = string.Empty;
        public string CtaImponible1 { get; set; } = string.Empty;
        public string CtaIgv1 { get; set; } = string.Empty;
        public string CtaImponible2 { get; set; } = string.Empty;
        public string CtaIgv2 { get; set; } = string.Empty;
        public string CtaImponible3 { get; set; } = string.Empty;
        public string CtaIgv3 { get; set; } = string.Empty;
        public string CtaImponible4 { get; set; } = string.Empty;
        public string CtaIgv4 { get; set; } = string.Empty;
        public string CtaImponible5 { get; set; } = string.Empty;
        public string CtaIgv5 { get; set; } = string.Empty;
        public string CtaTotal { get; set; } = string.Empty;
    }

}