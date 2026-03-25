namespace Inmobiliaria_KapiConta.Models
{
    public class PlanCuentaItem
    {
        public int IdPlanCuenta { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdPlanCuentaBase { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Nivel { get; set; }
        public string CodigoPadre { get; set; }
        public int IdElemento { get; set; }
        public int? IdBalance { get; set; }
        public bool Analisis { get; set; }
        public bool EsBase { get; set; }
        public bool TieneAutomatizacion { get; set; }
        public bool Estado { get; set; }
    }
}
