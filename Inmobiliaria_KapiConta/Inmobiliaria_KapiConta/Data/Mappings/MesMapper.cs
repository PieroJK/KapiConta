using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class MesMapper
    {
        public static Mes Map(NpgsqlDataReader dr)
        {
            return new Mes
            {
                IdMes = Convert.ToInt32(dr["id_mes"]),
                mes = dr["mes"]?.ToString() ?? string.Empty,
                Nombre = dr["nombre"]?.ToString() ?? string.Empty
            };
        }
    }
}