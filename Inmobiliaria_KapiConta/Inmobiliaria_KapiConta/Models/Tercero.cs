namespace Inmobiliaria_KapiConta.Models
{
    public class Tercero
    {
        public int IdTercero { get; set; }

        public string Documento { get; set; } = "";
        public string RazonSocial { get; set; } = "";
        public string? Direccion { get; set; }

        public int IdTerceroTipoDocumento { get; set; }
        public string? TipoDocumentoNombre { get; set; } // 👈 para mostrar en UI

        public string? Condicion { get; set; }
        public bool Estado { get; set; } = true;
    }
}