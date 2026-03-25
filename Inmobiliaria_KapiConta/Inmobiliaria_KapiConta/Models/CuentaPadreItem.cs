namespace Inmobiliaria_KapiConta.Models
{
    public class CuentaPadreItem
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public string Texto => $"{Codigo} - {Descripcion}";
    }
}
