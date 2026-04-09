using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class PeriodoMapper
    {
        public static Periodo Map(NpgsqlDataReader reader)
        {
            return new Periodo
            {
                IdPeriodo = (int)reader["id_periodo"],
                Anio = (int)reader["anio"],
                IdEmpresa = (int)reader["id_empresa"],

                // 🔥 Solo se llena si hiciste JOIN con empresa
                Empresa = reader.HasColumn("nombre") ? new Empresa
                {
                    IdEmpresa = (int)reader["id_empresa"],
                    Nombre = reader["nombre"]?.ToString() ?? string.Empty
                } : null
            };
        }
    }

    // 🔧 Extensión útil para validar columnas (evita errores en JOIN opcional)
    public static class DataReaderExtensions
    {
        public static bool HasColumn(this NpgsqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
