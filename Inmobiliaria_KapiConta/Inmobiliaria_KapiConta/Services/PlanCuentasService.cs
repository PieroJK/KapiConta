using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Models;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Inmobiliaria_KapiConta.Services
{
    public class PlanCuentasService
    {
        private readonly int _empresaId;

        public PlanCuentasService(int empresaId)
        {
            _empresaId = empresaId;
        }

        #region VALIDACIONES

        public bool TieneColumnaIdPlanCuentaBase()
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            try
            {
                string sql = @"
                    SELECT COUNT(*)
                    FROM information_schema.columns
                    WHERE table_schema = 'public'
                      AND table_name = 'plan_cuenta'
                      AND column_name = 'id_plan_cuenta_base';";

                using var cmd = new NpgsqlCommand(sql, cn);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool ExisteCodigo(string codigo, int? idExcluir = null)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                SELECT COUNT(*)
                FROM plan_cuenta
                WHERE id_empresa = @idEmpresa
                  AND codigo = @codigo";

            if (idExcluir.HasValue)
                sql += " AND id_plan_cuenta <> @idExcluir";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            if (idExcluir.HasValue)
                cmd.Parameters.AddWithValue("@idExcluir", idExcluir.Value);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        #endregion

        #region CONSULTAS

        public List<PlanCuentaItem> ObtenerPlanCuentas()
        {
            var lista = new List<PlanCuentaItem>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            bool tieneColumnaBase = TieneColumnaIdPlanCuentaBase();

            string sql = tieneColumnaBase
                ? @"SELECT id_plan_cuenta, id_empresa, id_plan_cuenta_base,
                           codigo, descripcion, nivel, codigo_padre,
                           id_elemento, id_balance, analisis, es_base,
                           tiene_automatizacion, estado
                    FROM plan_cuenta
                    WHERE id_empresa = @idEmpresa AND estado = true
                    ORDER BY codigo;"
                : @"SELECT id_plan_cuenta, id_empresa,
                           NULL::int AS id_plan_cuenta_base,
                           codigo, descripcion, nivel, codigo_padre,
                           id_elemento, id_balance, analisis, es_base,
                           tiene_automatizacion, estado
                    FROM plan_cuenta
                    WHERE id_empresa = @idEmpresa AND estado = true
                    ORDER BY codigo;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new PlanCuentaItem
                {
                    IdPlanCuenta = Convert.ToInt32(dr["id_plan_cuenta"]),
                    IdEmpresa = Convert.ToInt32(dr["id_empresa"]),
                    IdPlanCuentaBase = dr["id_plan_cuenta_base"] == DBNull.Value ? null : Convert.ToInt32(dr["id_plan_cuenta_base"]),
                    Codigo = dr["codigo"]?.ToString(),
                    Descripcion = dr["descripcion"]?.ToString(),
                    Nivel = Convert.ToInt32(dr["nivel"]),
                    CodigoPadre = dr["codigo_padre"] == DBNull.Value ? null : dr["codigo_padre"].ToString(),
                    IdElemento = Convert.ToInt32(dr["id_elemento"]),
                    IdBalance = dr["id_balance"] == DBNull.Value ? null : Convert.ToInt32(dr["id_balance"]),
                    Analisis = Convert.ToBoolean(dr["analisis"]),
                    EsBase = Convert.ToBoolean(dr["es_base"]),
                    TieneAutomatizacion = Convert.ToBoolean(dr["tiene_automatizacion"]),
                    Estado = Convert.ToBoolean(dr["estado"])
                });
            }

            return lista;
        }

        public List<CuentaPadreItem> ObtenerCuentasPadre()
        {
            var lista = new List<CuentaPadreItem>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
        SELECT codigo, descripcion
        FROM plan_cuenta
        WHERE id_empresa = @idEmpresa
          AND estado = true
        ORDER BY codigo;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new CuentaPadreItem
                {
                    Codigo = dr["codigo"]?.ToString(),
                    Descripcion = dr["descripcion"]?.ToString()
                });
            }

            return lista;
        }

        #endregion

        #region CRUD

        public void InsertarCuenta(PlanCuentaItem item)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            bool tieneColumnaBase = TieneColumnaIdPlanCuentaBase();

            string sql = tieneColumnaBase
                ? @"INSERT INTO plan_cuenta
                   (id_empresa, id_plan_cuenta_base, codigo, descripcion, nivel,
                    codigo_padre, id_elemento, id_balance, analisis,
                    es_base, tiene_automatizacion, estado)
                   VALUES
                   (@idEmpresa, @idPlanCuentaBase, @codigo, @descripcion, @nivel,
                    @codigoPadre, @idElemento, @idBalance, @analisis,
                    false, false, true);"
                : @"INSERT INTO plan_cuenta
                   (id_empresa, codigo, descripcion, nivel,
                    codigo_padre, id_elemento, id_balance, analisis,
                    es_base, tiene_automatizacion, estado)
                   VALUES
                   (@idEmpresa, @codigo, @descripcion, @nivel,
                    @codigoPadre, @idElemento, @idBalance, @analisis,
                    false, false, true);";

            using var cmd = new NpgsqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);
            cmd.Parameters.AddWithValue("@codigo", item.Codigo);
            cmd.Parameters.AddWithValue("@descripcion", item.Descripcion);
            cmd.Parameters.AddWithValue("@nivel", item.Nivel);
            cmd.Parameters.AddWithValue("@codigoPadre",
                string.IsNullOrWhiteSpace(item.CodigoPadre) ? (object)DBNull.Value : item.CodigoPadre);
            cmd.Parameters.AddWithValue("@idElemento", item.IdElemento);
            cmd.Parameters.AddWithValue("@idBalance",
                item.IdBalance.HasValue ? item.IdBalance.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@analisis", item.Analisis);

            if (tieneColumnaBase)
                cmd.Parameters.AddWithValue("@idPlanCuentaBase",
                    item.IdPlanCuentaBase.HasValue ? item.IdPlanCuentaBase.Value : (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public void ModificarCuenta(PlanCuentaItem item)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                UPDATE plan_cuenta
                SET codigo = @codigo,
                    descripcion = @descripcion,
                    nivel = @nivel,
                    codigo_padre = @codigoPadre,
                    id_elemento = @idElemento,
                    id_balance = @idBalance,
                    analisis = @analisis
                WHERE id_plan_cuenta = @idPlanCuenta
                  AND id_empresa = @idEmpresa;";

            using var cmd = new NpgsqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@codigo", item.Codigo);
            cmd.Parameters.AddWithValue("@descripcion", item.Descripcion);
            cmd.Parameters.AddWithValue("@nivel", item.Nivel.ToString());
            cmd.Parameters.AddWithValue("@codigoPadre",
                string.IsNullOrWhiteSpace(item.CodigoPadre) ? (object)DBNull.Value : item.CodigoPadre);
            cmd.Parameters.AddWithValue("@idElemento", item.IdElemento);
            cmd.Parameters.AddWithValue("@idBalance",
                item.IdBalance.HasValue ? item.IdBalance.Value : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@analisis", item.Analisis);
            cmd.Parameters.AddWithValue("@idPlanCuenta", item.IdPlanCuenta);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            cmd.ExecuteNonQuery();
        }

        public void EliminarCuenta(int idPlanCuenta)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                UPDATE plan_cuenta
                SET estado = false
                WHERE id_plan_cuenta = @idPlanCuenta
                  AND id_empresa = @idEmpresa;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            cmd.ExecuteNonQuery();
        }

        

        #endregion
    }
}