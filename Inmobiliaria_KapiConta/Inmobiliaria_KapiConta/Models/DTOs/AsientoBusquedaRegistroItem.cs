namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class AsientoBusquedaRegistroItem
    {
        public int IdAsiento { get; set; }

        public string Referencia { get; set; } = string.Empty;

        public string MesCodigo { get; set; } = string.Empty;

        public string SubDiarioCodigo { get; set; } = string.Empty;

        public string FechaTexto { get; set; } = string.Empty;

        public string Glosa { get; set; } = string.Empty;

        public decimal Importe { get; set; }

        public string Usuario { get; set; } = string.Empty;
    }
}
