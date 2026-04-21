namespace Inmobiliaria_KapiConta.Models
{
    public class RelacionAsiento
    {
        public int IdRelacion { get; set; }
        public int AsientoOrigen { get; set; }
        public int AsientoRelacionado { get; set; }
        public bool Estado { get; set; } = true;

        // Navegación
        public Asiento? Origen { get; set; }
        public Asiento? Relacionado { get; set; }
    }
}
