using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class PendienteItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public string Asiento { get; set; }
        public string Cuenta { get; set; }
        public int IdPlanCuenta { get; set; }
        public string Documento { get; set; }
        public int? IdTercero { get; set; }
        public string Ruc { get; set; }
        public string Proveedor { get; set; }
        public decimal MontoOriginal { get; set; }
        public decimal Saldo { get; set; }

        // =========================
        // SELECCIÓN (CHECKBOX)
        // =========================

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }
    }
}