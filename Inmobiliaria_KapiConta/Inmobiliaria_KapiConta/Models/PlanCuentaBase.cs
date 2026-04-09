namespace Inmobiliaria_KapiConta.Models
{
    public class PlanCuentaBase
    {
        public int IdPlanCuentaBase { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Nivel { get; set; } = string.Empty;

        // 🔥 Relación consigo misma (padre)
        public string? CodigoPadre { get; set; }

        // 🔥 Claves foráneas
        public int IdElemento { get; set; }

        public int? IdBalance { get; set; }

        // 🔥 Propiedades de navegación
        public Elemento? Elemento { get; set; }

        public Balance? Balance { get; set; }

        // 🔥 Autorreferencia (padre)
        public PlanCuentaBase? CuentaPadre { get; set; }

        public bool Analisis { get; set; }

        public bool Estado { get; set; } = true;

        // 🔥 Para mostrar en UI
        public override string ToString()
        {
            return $"{Codigo} - {Descripcion}";
        }
    }
}
