using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class AsientoDetalleMapper
    {
        public static AsientoDetalle Map(NpgsqlDataReader dr)
        {
            var detalle = new AsientoDetalle
            {
                IdAsientoDetalle = Convert.ToInt32(dr["id_asiento_detalle"]),
                IdAsiento = Convert.ToInt32(dr["id_asiento"]),
                IdPlanCuenta = Convert.ToInt32(dr["id_plan_cuenta"]),
                Moneda = dr["moneda"]?.ToString() ?? "PEN",
                Debe = Convert.ToDecimal(dr["debe"]),
                Haber = Convert.ToDecimal(dr["haber"]),
                Glosa = dr["glosa"]?.ToString() ?? string.Empty,
                IdTipoFacturacion = dr["id_tipo_facturacion"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_tipo_facturacion"]) : null,
                IdTercero = dr["id_tercero"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_tercero"]) : null,
                IdRelacion = dr["id_relacion"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_relacion"]) : null,
                IdTipoOperacion = dr["id_tipo_operacion"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_tipo_operacion"]) : null,
                IdCosto = dr["id_costo"] != DBNull.Value
                                        ? Convert.ToInt32(dr["id_costo"]) : null,
                SerieComprobante = dr["serie_comprobante"] != DBNull.Value
                                        ? dr["serie_comprobante"].ToString() : null
            };

            // PlanCuenta
            if (dr.HasColumn("plan_cuenta_codigo"))
                detalle.PlanCuenta = new PlanCuenta
                {
                    IdPlanCuenta = detalle.IdPlanCuenta,
                    Codigo = dr["plan_cuenta_codigo"]?.ToString() ?? string.Empty,
                    Descripcion = dr["plan_cuenta_descripcion"]?.ToString() ?? string.Empty
                };

            // TipoFacturacion
            if (dr.HasColumn("tipo_facturacion_nombre") && detalle.IdTipoFacturacion != null)
                detalle.TipoFacturacion = new TipoFacturacion
                {
                    IdTipoFacturacion = detalle.IdTipoFacturacion.Value,
                    Cod = dr["tipo_facturacion_cod"]?.ToString() ?? string.Empty,
                    Nombre = dr["tipo_facturacion_nombre"]?.ToString() ?? string.Empty
                };

            // Tercero
            if (dr.HasColumn("tercero_razon_social") && detalle.IdTercero != null)
                detalle.Tercero = new Tercero
                {
                    IdTercero = detalle.IdTercero.Value,
                    RazonSocial = dr["tercero_razon_social"]?.ToString() ?? string.Empty,
                    Documento = dr["tercero_documento"]?.ToString() ?? string.Empty
                };

            // TipoOperacion
            if (dr.HasColumn("tipo_operacion_nombre") && detalle.IdTipoOperacion != null)
                detalle.TipoOperacion = new TipoOperacionAsiento
                {
                    IdTipoOperacion = detalle.IdTipoOperacion.Value,
                    Codigo = dr["tipo_operacion_codigo"]?.ToString() ?? string.Empty,
                    Nombre = dr["tipo_operacion_nombre"]?.ToString() ?? string.Empty
                };

            // Costo
            if (dr.HasColumn("costo_codigo") && detalle.IdCosto != null)
                detalle.Costo = new Costo
                {
                    IdCosto = detalle.IdCosto.Value,
                    Codigo = dr["costo_codigo"]?.ToString() ?? string.Empty,
                    Descripcion = dr["costo_descripcion"]?.ToString() ?? string.Empty
                };

            return detalle;
        }
    }
}
