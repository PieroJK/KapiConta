using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class AsientoMapper
    {
        public static Asiento Map(NpgsqlDataReader dr)
        {
            var asiento = new Asiento
            {
                IdAsiento = Convert.ToInt32(dr["id_asiento"]),
                IdEmpresa = Convert.ToInt32(dr["id_empresa"]),
                IdMes = Convert.ToInt32(dr["id_mes"]),
                IdSubDiario = Convert.ToInt32(dr["id_sub_diario"]),
                IdLibro = Convert.ToInt32(dr["id_libro"]),
                IdPeriodo = Convert.ToInt32(dr["id_periodo"]),
                IdTipoCambio = dr["id_tipo_cambio"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_tipo_cambio"]) : null,
                IdUsuario = dr["id_usuario"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_usuario"]) : null,
                Referencia = dr["referencia"] != DBNull.Value
                                        ? dr["referencia"].ToString() : null,
                Fecha = Convert.ToDateTime(dr["fecha"]),
                Moneda = dr["moneda"]?.ToString() ?? "PEN",
                FechaVen = dr["fecha_ven"] != DBNull.Value
                                        ? Convert.ToDateTime(dr["fecha_ven"]) : null,
                FechaModificacion = Convert.ToDateTime(dr["fecha_modificacion"]),
                Estado = Convert.ToBoolean(dr["estado"])
            };

            // Empresa
            if (dr.HasColumn("empresa_nombre"))
                asiento.Empresa = new Empresa
                {
                    IdEmpresa = asiento.IdEmpresa,
                    Nombre = dr["empresa_nombre"]?.ToString() ?? string.Empty
                };

            // Mes
            if (dr.HasColumn("mes_nombre"))
                asiento.Mes = new Mes
                {
                    IdMes = asiento.IdMes,
                    Nombre = dr["mes_nombre"]?.ToString() ?? string.Empty
                };

            // SubDiario
            if (dr.HasColumn("sub_diario_nombre"))
                asiento.SubDiario = new SubDiario
                {
                    IdSubDiario = asiento.IdSubDiario,
                    Nombre = dr["sub_diario_nombre"]?.ToString() ?? string.Empty
                };

            // Libro
            if (dr.HasColumn("libro_nombre"))
                asiento.Libro = new Libro
                {
                    IdLibro = asiento.IdLibro,
                    Nombre = dr["libro_nombre"]?.ToString() ?? string.Empty
                };

            // TipoCambio
            if (dr.HasColumn("tipo_cambio_fecha") && asiento.IdTipoCambio != null)
                asiento.TipoCambio = new TipoCambio
                {
                    IdTipoCambio = asiento.IdTipoCambio.Value,
                    Fecha = Convert.ToDateTime(dr["tipo_cambio_fecha"]),
                    Compra = dr["tipo_cambio_compra"] != DBNull.Value
                                    ? Convert.ToDecimal(dr["tipo_cambio_compra"]) : null,
                    Venta = dr["tipo_cambio_venta"] != DBNull.Value
                                    ? Convert.ToDecimal(dr["tipo_cambio_venta"]) : null
                };

            // Usuario
            if (dr.HasColumn("usuario_nombre") && asiento.IdUsuario != null)
                asiento.Usuario = new Usuario
                {
                    Id = asiento.IdUsuario.Value,  // ✅ usa Id
                    Username = dr["usuario_nombre"]?.ToString() ?? string.Empty
                };

            // Periodo
            if (dr.HasColumn("periodo_anio"))
                asiento.Periodo = new Periodo
                {
                    IdPeriodo = asiento.IdPeriodo,
                    Anio = Convert.ToInt32(dr["periodo_anio"])
                };

            return asiento;
        }
    }
}
