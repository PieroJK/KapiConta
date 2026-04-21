namespace Inmobiliaria_KapiConta.Models
{
    public class AsientoDetalle
    {
        public int IdAsientoDetalle { get; set; }

        // FKs
        public int IdAsiento { get; set; }
        public int IdPlanCuenta { get; set; }
        public int? IdTipoFacturacion { get; set; }
        public int? IdTercero { get; set; }
        public int? IdRelacion { get; set; }
        public int? IdTipoOperacion { get; set; }
        public int? IdCosto { get; set; }

        // Campos propios
        public string Moneda { get; set; } = "PEN";
        public decimal Debe { get; set; } = 0;
        public decimal Haber { get; set; } = 0;
        public string? SerieComprobante { get; set; }
        public string Glosa { get; set; } = string.Empty;

        // Navegación
        public Asiento? Asiento { get; set; }
        public PlanCuenta? PlanCuenta { get; set; }
        public TipoFacturacion? TipoFacturacion { get; set; }
        public Tercero? Tercero { get; set; }
        public RelacionAsiento? Relacion { get; set; }
        public TipoOperacionAsiento? TipoOperacion { get; set; }
        public Costo? Costo { get; set; }
    }
}