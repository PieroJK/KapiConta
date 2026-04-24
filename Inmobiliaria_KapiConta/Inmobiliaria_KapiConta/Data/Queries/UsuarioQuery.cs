using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class UsuarioQuery
    {
        public static string Listar = 
            @"
            SELECT 
                    u.id_usuario,
                    u.usuario AS Username,
                    u.clave_hash AS PasswordHash,
                    u.nombre AS Nombre,
                    u.estado AS Estado,
                    r.id_rol AS IdRol,
                    r.nombre 
            FROM usuario u
            INNER JOIN rol r ON u.id_rol = r.id_rol
            ORDER BY u.usuario
            ;";

        public static string Insertar = 
            @"
            INSERT INTO usuario (usuario, clave_hash, nombre, id_rol, estado)
            VALUES (@usuario, @clave, @nombre, @rol, true)
            ;";

        public static string Modificar = 
            @"
            UPDATE usuario
            SET 
                    usuario = @usuario,
                    nombre = @nombre,
                    id_rol = @idRol,
                    estado = @estado,
                    clave_hash = @password
            WHERE id_usuario = @idUsuario
            ;";

        public static string Eliminar =
            @"
            UPDATE usuario
            SET estado = false
            WHERE id_usuario = @id
            ;";
        

    }
}
