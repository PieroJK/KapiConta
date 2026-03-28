namespace Inmobiliaria_KapiConta.Models
{
    public class AutomatizacionDetalleItem
    {
        public int? IdDetalle { get; set; }

        public int IdCuentaRelacionada { get; set; }

        public string CuentaCodigo { get; set; }

        public string CuentaDescripcion { get; set; }

        public string TipoMovimiento { get; set; } // D o H

        public decimal Porcentaje { get; set; }

        public decimal PorcentajeDebe => TipoMovimiento == "D" ? Porcentaje : 0;
        public decimal PorcentajeHaber => TipoMovimiento == "H" ? Porcentaje : 0;

        public string CodigoCuenta { get; set; } = "";
        public string NombreCuenta { get; set; } = "";


    }
}