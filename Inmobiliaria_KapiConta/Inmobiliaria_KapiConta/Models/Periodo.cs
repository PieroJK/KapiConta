namespace Inmobiliaria_KapiConta.Models
{
    public class Periodo
    {
        public int IdPeriodo { get; set; }
        public int Anio { get; set; }

        public override string ToString()
        {
            return Anio.ToString();
        }
    }
}
