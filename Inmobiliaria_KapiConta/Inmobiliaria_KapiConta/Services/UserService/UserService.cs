using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var lista = conn.Query<Usuario>(UsuarioQuery.Listar);
                Debug.WriteLine($"Cantidad de la lista: {lista.Count()}");
                Debug.WriteLine($"Tipo de variable: {lista.GetType}");
                return (List<Usuario>)lista;
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
