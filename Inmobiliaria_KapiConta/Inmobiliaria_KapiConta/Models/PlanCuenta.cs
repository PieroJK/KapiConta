namespace Inmobiliaria_KapiConta.Models
{
    public class PlanCuenta
    {
        public int IdPlanCuenta { get; set; }

        // 🔥 Relación con empresa
        public int IdEmpresa { get; set; }
        public Empresa? Empresa { get; set; }

        // 🔥 Relación con plan base
        public int? IdPlanCuentaBase { get; set; }
        public PlanCuentaBase? PlanCuentaBase { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Nivel { get; set; } = string.Empty;

        // 🔥 Autorreferencia (padre dentro de la misma empresa)
        public string? CodigoPadre { get; set; }
        public PlanCuenta? CuentaPadre { get; set; }

        // 🔥 Relaciones
        public int IdElemento { get; set; }
        public Elemento? Elemento { get; set; }

        public int? IdBalance { get; set; }
        public Balance? Balance { get; set; }

        public bool Analisis { get; set; }

        public bool EsBase { get; set; } = false;

        public bool TieneAutomatizacion { get; set; } = false;

        public bool Estado { get; set; } = true;

        // 🔥 Para UI (muy útil)
        public override string ToString()
        {
            return $"{Codigo} - {Descripcion}";
        }
    }
}