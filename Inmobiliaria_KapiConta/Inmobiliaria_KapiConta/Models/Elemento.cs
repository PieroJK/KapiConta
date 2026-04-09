namespace Inmobiliaria_KapiConta.Models
{
    public class Elemento
    {
        public int IdElemento { get; set; }

        public string Nombre { get; set; } = string.Empty;

        // 🔥 Para mostrar en ComboBox, DataGrid, etc.
        public override string ToString()
        {
            return Nombre;
        }
    }
}
