namespace Inmobiliaria_KapiConta.Models
{
    public class CuentaAutomatizacionDetalle
    {
        public int IdDetalle { get; set; }

        public int IdAutomatizacion { get; set; }

        public int IdCuentaRelacionada { get; set; }

        // 🔥 Relación
        public PlanCuenta? CuentaRelacionada { get; set; }

        public string TipoMovimiento { get; set; } = string.Empty; // D / H

        public decimal Porcentaje { get; set; }
    }
}
