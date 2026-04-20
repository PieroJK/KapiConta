using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class TerceroMapper
    {
        public static Tercero Map(NpgsqlDataReader dr)
        {
            return new Tercero
            {
                IdTercero = Convert.ToInt32(dr["id_tercero"]),
                Documento = dr["documento"]?.ToString() ?? "",
                RazonSocial = dr["razon_social"]?.ToString() ?? "",
                Direccion = dr["direccion"] == DBNull.Value ? null : dr["direccion"].ToString(),
                Estado = Convert.ToBoolean(dr["estado"]),
                Condicion = dr["condicion"] == DBNull.Value ? null : dr["condicion"].ToString(),
                IdTerceroTipoDocumento = Convert.ToInt32(dr["id_tercero_tipo_documento"]),
                TipoDocumentoNombre = dr.HasColumn("tipo_documento")
                    ? dr["tipo_documento"]?.ToString()
                    : null
            };
        }

        public static TerceroTipoDocumento MapTipo(NpgsqlDataReader dr)
        {
            return new TerceroTipoDocumento
            {
                IdTerceroTipoDocumento = Convert.ToInt32(dr["id_tercero_tipo_documento"]),
                Cod = Convert.ToInt32(dr["cod"]),
                Nombre = dr["nombre"]?.ToString() ?? ""
            };
        }
    }
}