using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class SubDiarioMapper
    {
        public static SubDiario Map(NpgsqlDataReader dr)
        {
            return new SubDiario
            {
                IdSubDiario = Convert.ToInt32(dr["id_sub_diario"]),
                Diario = dr["diario"]?.ToString() ?? string.Empty,
                Nombre = dr["nombre"]?.ToString() ?? string.Empty
            };
        }
    }
}