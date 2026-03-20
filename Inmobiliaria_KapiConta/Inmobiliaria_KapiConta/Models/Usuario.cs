namespace Inmobiliaria_KapiConta.Models
{
	public class Usuario
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string PasswordHash { get; set; }
		public string Nombre { get; set; }
		public bool Estado { get; set; }

		// ?? Relación
		public RolUsuario Rol { get; set; }
	}
}