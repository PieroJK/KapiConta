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

namespace Inmobiliaria_KapiConta.Services.RolService
{
    public class RolService: IRolService
    {
        public List<RolUsuario> LoadRoles()
        {
            using var conn = DbConnectionFactory.Create();
            var Roles = conn.Query<RolUsuario>(RolesQuery.Listar).ToList();
            Debug.WriteLine($"Roles cargados: {Roles.Count}");
            foreach (var item in Roles)
            {
                Debug.WriteLine($"ID Rol: {item.IdRol}");
                Debug.WriteLine($"Nombre Rol: {item.Nombre}");
            }
            return Roles;
        }

        
    }
}
