namespace Inmobiliaria_KapiConta.Models
{
    public class TipoOperacionAsiento
    {
        public int IdTipoOperacion { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;
    }
}
