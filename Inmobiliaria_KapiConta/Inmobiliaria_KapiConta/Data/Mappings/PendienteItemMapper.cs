using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inmobiliaria_KapiConta.Models.DTOs;
using Npgsql;

namespace Inmobiliaria_KapiConta.Data.Mappings
{
    public static class PendienteItemMapper
    {
        public static PendienteItem Map(NpgsqlDataReader dr)
        {
            decimal saldoActual = Convert.ToDecimal(dr["saldo_actual"]);
            decimal montoOrig = Convert.ToDecimal(dr["monto_original"]);

            return new PendienteItem
            {
                Asiento = dr["asiento_ref"]?.ToString() ?? "",
                Cuenta = dr["cuenta_cod"]?.ToString() ?? "",
                IdPlanCuenta = Convert.ToInt32(dr["id_plan_cuenta"]),
                Documento = dr["doc_nro"]?.ToString() ?? "",
                IdTercero = dr["id_tercero"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(dr["id_tercero"]),
                Ruc = dr["ruc_tercero"]?.ToString() ?? "",
                Proveedor = dr["tercero_nom"]?.ToString() ?? "SIN NOMBRE",
                MontoOriginal = Math.Abs(montoOrig),
                Saldo = saldoActual
            };
        }
    }
}
