using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    // Los mappers se utlizan para convertir objetos de un tipo a otro tipo.
    public static class EmpresaMapper
    {
        public static Empresa Map(NpgsqlDataReader reader)
        {
            return new Empresa
            {
                IdEmpresa = (int)reader["id_empresa"],
                Ruc = reader["ruc"]?.ToString() ?? string.Empty,
                Nombre = reader["nombre"]?.ToString() ?? string.Empty,
                Direccion = reader["direccion"]?.ToString() ?? string.Empty,
                Estado = (bool)reader["estado"]
            };
        }
    }
}
