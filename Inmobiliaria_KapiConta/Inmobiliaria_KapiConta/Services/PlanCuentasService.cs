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

        public (List<AutomatizacionDetalleItem> lista, bool estado) ObtenerAutomatizacion(int idPlanCuenta)
        {
            var lista = new List<AutomatizacionDetalleItem>();
            bool estadoAutomatizacion = false;

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            // 🔹 1. Obtener estado
            string sqlEstado = @"
        SELECT estado
        FROM cuenta_automatizacion
        WHERE id_plan_cuenta = @idPlanCuenta
        ORDER BY id_automatizacion DESC
        LIMIT 1;";

            using (var cmdEstado = new NpgsqlCommand(sqlEstado, cn))
            {
                cmdEstado.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

                object result = cmdEstado.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    estadoAutomatizacion = Convert.ToBoolean(result);
            }

            // 🔹 2. Obtener detalle COMPLETO (igual que antes)
            string sql = @"
        SELECT 
            cad.id_detalle,
            cad.id_cuenta_relacionada,
            pc.codigo,
            pc.descripcion,
            cad.tipo_movimiento,
            cad.porcentaje
        FROM cuenta_automatizacion ca
        INNER JOIN cuenta_automatizacion_detalle cad
            ON ca.id_automatizacion = cad.id_automatizacion
        INNER JOIN plan_cuenta pc
            ON pc.id_plan_cuenta = cad.id_cuenta_relacionada
        WHERE ca.id_plan_cuenta = @idPlanCuenta
        ORDER BY cad.id_detalle;";

            using (var cmd = new NpgsqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new AutomatizacionDetalleItem
                    {
                        IdDetalle = Convert.ToInt32(dr["id_detalle"]),
                        IdCuentaRelacionada = Convert.ToInt32(dr["id_cuenta_relacionada"]),
                        CuentaCodigo = dr["codigo"]?.ToString(),
                        CuentaDescripcion = dr["descripcion"]?.ToString(),
                        TipoMovimiento = dr["tipo_movimiento"]?.ToString(),
                        Porcentaje = Convert.ToDecimal(dr["porcentaje"])
                    });
                }
            }

            return (lista, estadoAutomatizacion);
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

        public void GuardarAutomatizacion(
    int idPlanCuenta,
    List<AutomatizacionDetalleItem> detalles,
    bool activo)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var tx = cn.BeginTransaction();

            try
            {
                // 🔹 1. Obtener automatización existente
                string sqlGet = @"
            SELECT id_automatizacion
            FROM cuenta_automatizacion
            WHERE id_plan_cuenta = @idPlanCuenta
            ORDER BY id_automatizacion DESC
            LIMIT 1;";

                int? idAutomatizacion = null;

                using (var cmd = new NpgsqlCommand(sqlGet, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                        idAutomatizacion = Convert.ToInt32(result);
                }

                // 🔴 CASO 1: DESACTIVAR (igual que antes)
                if (!activo)
                {
                    if (idAutomatizacion.HasValue)
                    {
                        string sqlDesactivar = @"
                    UPDATE cuenta_automatizacion
                    SET estado = false
                    WHERE id_automatizacion = @id;";

                        using var cmd = new NpgsqlCommand(sqlDesactivar, cn, tx);
                        cmd.Parameters.AddWithValue("@id", idAutomatizacion.Value);
                        cmd.ExecuteNonQuery();
                    }

                    // actualizar flag en plan_cuenta
                    string updateFlag = @"
                UPDATE plan_cuenta
                SET tiene_automatizacion = false
                WHERE id_plan_cuenta = @id;";

                    using (var cmd = new NpgsqlCommand(updateFlag, cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", idPlanCuenta);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return;
                }

                // 🔹 2. Validación básica (por seguridad backend)
                if (detalles == null || detalles.Count == 0)
                    throw new Exception("No hay detalles de automatización.");

                // 🔹 3. Crear si no existe
                int idFinal;

                if (!idAutomatizacion.HasValue)
                {
                    string insert = @"
                INSERT INTO cuenta_automatizacion (id_plan_cuenta, estado)
                VALUES (@idPlanCuenta, true)
                RETURNING id_automatizacion;";

                    using var cmdInsert = new NpgsqlCommand(insert, cn, tx);
                    cmdInsert.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);

                    idFinal = Convert.ToInt32(cmdInsert.ExecuteScalar());
                }
                else
                {
                    idFinal = idAutomatizacion.Value;
                }

                // 🔹 4. ACTIVAR automatización
                string sqlActivar = @"
            UPDATE cuenta_automatizacion
            SET estado = true
            WHERE id_automatizacion = @id;";

                using (var cmd = new NpgsqlCommand(sqlActivar, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", idFinal);
                    cmd.ExecuteNonQuery();
                }

                // 🔹 5. BORRAR DETALLES
                string delete = @"
            DELETE FROM cuenta_automatizacion_detalle
            WHERE id_automatizacion = @id;";

                using (var cmd = new NpgsqlCommand(delete, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", idFinal);
                    cmd.ExecuteNonQuery();
                }

                // 🔹 6. INSERTAR DETALLES
                string insertDetalle = @"
            INSERT INTO cuenta_automatizacion_detalle
            (id_automatizacion, id_cuenta_relacionada, tipo_movimiento, porcentaje)
            VALUES (@idAuto, @idCuenta, @tipo, @porcentaje);";

                foreach (var item in detalles)
                {
                    using var cmd = new NpgsqlCommand(insertDetalle, cn, tx);
                    cmd.Parameters.AddWithValue("@idAuto", idFinal);
                    cmd.Parameters.AddWithValue("@idCuenta", item.IdCuentaRelacionada);
                    cmd.Parameters.AddWithValue("@tipo", item.TipoMovimiento);
                    cmd.Parameters.AddWithValue("@porcentaje", item.Porcentaje);
                    cmd.ExecuteNonQuery();
                }

                // 🔹 7. ACTUALIZAR FLAG
                string update = @"
            UPDATE plan_cuenta
            SET tiene_automatizacion = true
            WHERE id_plan_cuenta = @id;";

                using (var cmd = new NpgsqlCommand(update, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", idPlanCuenta);
                    cmd.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        #endregion
    }
}