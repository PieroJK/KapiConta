using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class AuthService
    {
        public Usuario Login(string usuario, string password)
        {
            using var conn = DbConnectionFactory.Create();
            conn.Open();

            string query = @"
                SELECT 
                    u.id_usuario,
                    u.usuario,
                    u.clave_hash,
                    u.nombre,
                    u.estado,

                    r.id_rol,
                    r.nombre AS rol_nombre,
                    r.descripcion,
                    r.estado AS rol_estado

                FROM usuario u
                INNER JOIN rol_usuario r ON u.id_rol = r.id_rol
                WHERE u.usuario = @usuario
            ";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@usuario", usuario);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            string hash = reader["clave_hash"].ToString();

            if (!BCrypt.Net.BCrypt.Verify(password, hash))
                return null;

            //  MAPEO COMPLETO (usuario + rol)
            var user = new Usuario
            {
                Id = (int)reader["id_usuario"],
                Username = reader["usuario"].ToString(),
                PasswordHash = hash,
                Nombre = reader["nombre"].ToString(),
                Estado = (bool)reader["estado"],

                Rol = new RolUsuario
                {
                    IdRol = (int)reader["id_rol"],
                    Nombre = reader["rol_nombre"].ToString(),
                    Descripcion = reader["descripcion"].ToString(),
                    Estado = (bool)reader["rol_estado"]
                }
            };

            return user;
        }
    }
}