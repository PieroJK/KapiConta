namespace Inmobiliaria_KapiConta.Models
{
    public class Balance
    {
        public int IdBalance { get; set; }

        public string Nombre { get; set; } = string.Empty;

        // 🔥 Para mostrar en UI
        public override string ToString()
        {
            return Nombre;
        }
    }
}
