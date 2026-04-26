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
using Inmobiliaria_KapiConta.Services.PasswordService;
using Inmobiliaria_KapiConta.Services.RolService;

namespace Inmobiliaria_KapiConta.Services.UserService
{
    public class UserService : IUserService
    {

        private readonly IPasswordService _passwordService;
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
        public void AddUser(Usuario u, string plainTextPassword)
        {
            string hashedPassword = _passwordService.HashPassword(plainTextPassword);
            using var conn = DbConnectionFactory.Create();
            int rowAdded = conn.Execute(UsuarioQuery.Insertar, new
            {
                usuario = u.Username,
                clave = hashedPassword,
                nombre = u.Nombre,
                rol = u.Rol.IdRol
                //rol = rolId
            });
            Debug.WriteLine($"ROWS AFFECTED: {rowAdded}");
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
        public UserService(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }
    }
}
