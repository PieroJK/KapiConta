

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class AsientoCargadoItem
    {
        public int IdAsiento { get; set; }
        public int IdMes { get; set; }
        public int IdSubDiario { get; set; }
        public int IdLibro { get; set; }
        public string Referencia { get; set; } = "";
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; } = "PEN";
        public int? IdTipoCambio { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public List<AsientoDetalleCargadoItem> Detalles { get; set; } = new();
        public List<AsientoRelacionItem> Relaciones { get; set; } = new();
    }
}
