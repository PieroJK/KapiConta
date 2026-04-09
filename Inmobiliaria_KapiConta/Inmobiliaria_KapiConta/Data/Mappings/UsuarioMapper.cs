using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class UsuarioMapper
    {
        public static Usuario Map(NpgsqlDataReader reader)
        {
            return new Usuario
            {
                Id = (int)reader["id_usuario"],
                Username = reader["usuario"].ToString(),
                PasswordHash = reader["clave_hash"].ToString(),
                Nombre = reader["nombre"].ToString(),
                Estado = (bool)reader["estado"],

                Rol = new RolUsuario
                {
                    IdRol = (int)reader["id_rol"],
                    Nombre = reader["rol_nombre"].ToString()
                }
            };
        }   
    }
}