using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class ElementoMapper
    {
        public static Elemento Map(NpgsqlDataReader reader)
        {
            return new Elemento
            {
                IdElemento = (int)reader["id_elemento"],
                Nombre = reader["nombre"]?.ToString() ?? string.Empty
            };
        }
    }
}
