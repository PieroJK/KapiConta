namespace Inmobiliaria_KapiConta.Models
{
    public class Costo
    {
        public int IdCosto { get; set; }
        public int IdEmpresa { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Estado { get; set; } = true;

        // Navegación
        public Empresa? Empresa { get; set; }
    }
}
