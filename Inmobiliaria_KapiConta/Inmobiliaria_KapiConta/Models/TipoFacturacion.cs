namespace Inmobiliaria_KapiConta.Models
{
    public class TipoFacturacion
    {
        public int IdTipoFacturacion { get; set; }
        public string Cod { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
    }
}
