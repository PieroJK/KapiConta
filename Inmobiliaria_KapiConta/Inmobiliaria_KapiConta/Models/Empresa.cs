namespace Inmobiliaria_KapiConta.Models
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }

        public string Ruc { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public bool Estado { get; set; } = true;

        // 🔥 Para mostrar en ComboBox, DataGrid, etc.
        public override string ToString()
        {
            return Nombre;
        }
    }
}
