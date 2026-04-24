using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Dapper;
using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.Services.UserService
{
    public class UserService : IUserService
    {

        public List<Usuario> ListUser()
        {
                using var conn = DbConnectionFactory.Create();
            var lista = conn.Query<Usuario, RolUsuario, Usuario>(UsuarioQuery.Listar, (usuario, rol) =>
            {
                usuario.Rol = rol;
                return usuario;
            },
            splitOn: "idrol");
                
            return lista.ToList(); //La propiedad ToList() convierte la lista de tipo IEnumerable a List<Usuario> 
        }
        public void AddUser(Usuario u)
        {
            using var conn = DbConnectionFactory.Create();
            var id = conn.QuerySingle<int>(UsuarioQuery.Insertar, new
            {
                id_usuario = u.Id,
                usuario = u.Username,
                clave_hash = u.PasswordHash,
                nombre = u.Nombre,
                id_rol = u.Rol.IdRol 
            });
            u.Id = id;
        }

        public void UpdateUser(Usuario u)
        {
            using var conn = DbConnectionFactory.Create();
            var id = conn.QuerySingle(UsuarioQuery.Modificar, new
            {
                id_usuario = u.Id,
                usuario = u.Username,
                clave_hash = u.PasswordHash,
                nombre = u.Nombre,
                id_rol = u.Rol.IdRol 
            });
        }
        public void DeleteUser(Usuario u) 
        {
            using var conn = DbConnectionFactory.Create();
            var id = conn.QuerySingle(UsuarioQuery.Modificar, new
            {
                id_usuario = u.Id,
            });
        }
    }
}
