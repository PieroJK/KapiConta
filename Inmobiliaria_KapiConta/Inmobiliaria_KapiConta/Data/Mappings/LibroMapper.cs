using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class LibroMapper
    {
        public static Libro Map(NpgsqlDataReader dr)
        {
            return new Libro
            {
                IdLibro = Convert.ToInt32(dr["id_libro"]),
                Cod = dr["cod"]?.ToString() ?? string.Empty,
                Nombre = dr["nombre"]?.ToString() ?? string.Empty
            };
        }
    }
}