namespace Inmobiliaria_KapiConta.Models
{
    public class TipoCambio
    {
        public int IdTipoCambio { get; set; }
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; } = string.Empty;
        public decimal? Compra { get; set; }
        public decimal? Venta { get; set; }
        public bool Estado { get; set; } = true;
    }
}
