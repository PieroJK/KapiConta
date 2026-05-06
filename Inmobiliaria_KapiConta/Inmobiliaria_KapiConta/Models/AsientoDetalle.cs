using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inmobiliaria_KapiConta.Models
{
    public class AsientoDetalle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        // =========================
        // IDs
        // =========================
        public int IdAsientoDetalle { get; set; }
        public int IdAsiento { get; set; }
        public int IdPlanCuenta { get; set; }
        public int? IdTipoFacturacion { get; set; }
        public int? IdTercero { get; set; }
        public int? IdRelacion { get; set; }
        public int? IdTipoOperacion { get; set; }
        public int? IdCosto { get; set; }

        // =========================
        // CAMPOS CON NOTIFICACIÓN
        // =========================

        private string _moneda = "PEN";
        public string Moneda
        {
            get => _moneda;
            set { _moneda = value; OnPropertyChanged(); }
        }

        private decimal _debe = 0;
        public decimal Debe
        {
            get => _debe;
            set { _debe = value; OnPropertyChanged(); }
        }

        private decimal _haber = 0;
        public decimal Haber
        {
            get => _haber;
            set { _haber = value; OnPropertyChanged(); }
        }

        private string? _serieComprobante;
        public string? SerieComprobante
        {
            get => _serieComprobante;
            set { _serieComprobante = value; OnPropertyChanged(); }
        }

        private string _glosa = string.Empty;
        public string Glosa
        {
            get => _glosa;
            set { _glosa = value; OnPropertyChanged(); }
        }

        // =========================
        // NAVEGACIÓN
        // =========================
        public Asiento? Asiento { get; set; }
        public PlanCuenta? PlanCuenta { get; set; }
        public TipoFacturacion? TipoFacturacion { get; set; }
        public Tercero? Tercero { get; set; }
        public RelacionAsiento? Relacion { get; set; }
        public TipoOperacionAsiento? TipoOperacion { get; set; }
        public Costo? Costo { get; set; }
    }
}