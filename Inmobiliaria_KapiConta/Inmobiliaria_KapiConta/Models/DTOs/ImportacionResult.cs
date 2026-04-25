namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class ImportacionResult
    {
        public bool Exitoso { get; set; }
        public int AsientosImportados { get; set; }
        public List<string> Errores { get; set; } = new();
    }
}