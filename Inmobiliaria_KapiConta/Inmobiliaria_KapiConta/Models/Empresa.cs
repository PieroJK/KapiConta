namespace Inmobiliaria_KapiConta.Models
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public override string ToString()
        {
            return Nombre;
        }
    }
}
