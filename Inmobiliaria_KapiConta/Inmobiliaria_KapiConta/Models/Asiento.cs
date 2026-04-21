namespace Inmobiliaria_KapiConta.Models
{
    public class Asiento
    {
        public int IdAsiento { get; set; }

        // FKs
        public int IdEmpresa { get; set; }
        public int IdMes { get; set; }
        public int IdSubDiario { get; set; }
        public int IdLibro { get; set; }
        public int? IdTipoCambio { get; set; }
        public int? IdUsuario { get; set; }
        public int IdPeriodo { get; set; }

        // Campos propios
        public string? Referencia { get; set; }
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; } = "PEN";
        public DateTime? FechaVen { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Estado { get; set; } = true;

        // Navegación
        public Empresa? Empresa { get; set; }
        public Mes? Mes { get; set; }
        public SubDiario? SubDiario { get; set; }
        public Libro? Libro { get; set; }
        public TipoCambio? TipoCambio { get; set; }
        public Usuario? Usuario { get; set; }
        public Periodo? Periodo { get; set; }
    }
}
