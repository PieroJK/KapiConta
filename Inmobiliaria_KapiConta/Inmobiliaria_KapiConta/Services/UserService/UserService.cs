using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
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
            Debug.WriteLine($"USER SERVICE");
            foreach (var item in lista)
            {
                Debug.WriteLine($"Id: {item.Id}");
                Debug.WriteLine($"Username: {item.Username}");
                Debug.WriteLine($"Password: {item.PasswordHash}");
            }

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
            });
            Debug.WriteLine($"ROWS AFFECTED: {rowAdded}");
        }

        public void UpdateUser(Usuario u)
        {
            using var conn = DbConnectionFactory.Create();
            Debug.WriteLine($"PAYLOAD");
            Debug.WriteLine($"ID: {u.Id}");
            Debug.WriteLine($"USERNAME: {u.Username}");
            Debug.WriteLine($"PASSWORD: {u.PasswordHash}");
            Debug.WriteLine($"NAME: {u.Nombre}");
            Debug.WriteLine($"STATE: {u.Estado}");
            Debug.WriteLine($"ROLE: {u.Rol.Nombre}");
            Debug.WriteLine($"ROLE ID: {u.Rol.IdRol}");

            try
            {
                string hashedPassword = _passwordService.HashPassword(u.PasswordHash);
                var id = conn.QuerySingle(UsuarioQuery.Modificar, new
                {
                    idUsuario = u.Id,
                    usuario = u.Username,
                    password = hashedPassword,
                    nombre = u.Nombre,
                    idRol = u.Rol.IdRol,
                    estado = u.Estado
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al registrar usuario {ex}");
            }
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
