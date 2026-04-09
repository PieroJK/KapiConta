namespace Inmobiliaria_KapiConta.Models
{
    public class Periodo
    {
        public int IdPeriodo { get; set; }

        public int Anio { get; set; }

        // 🔥 Clave foránea
        public int IdEmpresa { get; set; }

        // 🔥 Propiedad de navegación (opcional pero recomendable)
        public Empresa? Empresa { get; set; }

        public override string ToString()
        {
            return Anio.ToString();
        }
    }
}
