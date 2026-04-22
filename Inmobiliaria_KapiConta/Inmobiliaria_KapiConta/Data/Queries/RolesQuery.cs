using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Data.Queries
{
    public class RolesQuery
    {
        public static string Listar = 
            @"
            SELECT 
                id_rol, nombre 
            FROM rol 
            ORDER BY nombre;";
    }
}
