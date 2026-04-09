namespace Inmobiliaria_KapiConta.Models
{
    public class CuentaAutomatizacion
    {
        public int IdAutomatizacion { get; set; }

        public int IdPlanCuenta { get; set; }

        // 🔥 Relación
        public PlanCuenta? PlanCuenta { get; set; }

        public bool Estado { get; set; } = true;

        // 🔥 Detalles
        public List<CuentaAutomatizacionDetalle> Detalles { get; set; } = new();
    }
}
