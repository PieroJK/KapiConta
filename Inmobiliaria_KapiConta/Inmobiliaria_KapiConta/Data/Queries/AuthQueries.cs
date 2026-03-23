namespace Inmobiliaria_KapiConta.Data.Queries
{
    public static class AuthQueries
    {
        public static string LoginQuery = @"
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
            INNER JOIN rol r ON u.id_rol = r.id_rol
            WHERE u.usuario = @usuario
        ";
    }
}