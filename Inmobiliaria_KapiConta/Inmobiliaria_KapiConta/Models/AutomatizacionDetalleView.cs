namespace Inmobiliaria_KapiConta.Models
{
    public class AutomatizacionDetalleView
    {
        public int IdDetalle { get; set; }
        public string? CuentaCodigo { get; set; }
        public decimal PorcentajeDebe { get; set; }
        public decimal PorcentajeHaber { get; set; }
    }
}
