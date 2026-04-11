using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;
using NpgsqlTypes;
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

        public bool ExisteCodigo(string codigo, int? idExcluir = null)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = idExcluir.HasValue
                ? PlanCuentaQueries.ExisteCodigoExcluyendo
                : PlanCuentaQueries.ExisteCodigo;

            using var cmd = new NpgsqlCommand(sql, cn);

            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            if (idExcluir.HasValue)
                cmd.Parameters.AddWithValue("@idExcluir", idExcluir.Value);

            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        #endregion

        #region CONSULTAS

        public List<PlanCuenta> ObtenerPlanCuentas()
        {
            var lista = new List<PlanCuenta>();

            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(PlanCuentaQueries.Listar, cn);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(PlanCuentaMapper.Map(dr));
            }

            return lista;
        }

        #endregion

        #region CRUD

        public PlanCuenta InsertarCuenta(PlanCuenta item)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(PlanCuentaQueries.Insertar, cn);

            // 🔍 DEBUG (puedes dejarlo temporal)
            Console.WriteLine($"Base: {item.IdPlanCuentaBase}");
            Console.WriteLine($"Codigo: {item.Codigo}");
            Console.WriteLine($"Balance: {item.IdBalance}");
            Console.WriteLine($"Padre: {item.CodigoPadre}");

            // ✅ PARÁMETROS TIPADOS (BIEN HECHO)
            cmd.Parameters.Add("@idEmpresa", NpgsqlTypes.NpgsqlDbType.Integer)
                .Value = _empresaId;

            cmd.Parameters.Add("@idPlanCuentaBase", NpgsqlTypes.NpgsqlDbType.Integer)
                .Value = (object?)item.IdPlanCuentaBase ?? DBNull.Value;

            cmd.Parameters.Add("@codigo", NpgsqlTypes.NpgsqlDbType.Varchar)
                .Value = item.Codigo;

            cmd.Parameters.Add("@descripcion", NpgsqlTypes.NpgsqlDbType.Varchar)
                .Value = item.Descripcion;

            // 🔥 IMPORTANTE: tu BD usa VARCHAR → no INTEGER
            cmd.Parameters.Add("@nivel", NpgsqlTypes.NpgsqlDbType.Varchar)
                .Value = item.Nivel.ToString();

            cmd.Parameters.Add("@codigoPadre", NpgsqlTypes.NpgsqlDbType.Varchar)
                .Value = (object?)item.CodigoPadre ?? DBNull.Value;

            cmd.Parameters.Add("@idElemento", NpgsqlTypes.NpgsqlDbType.Integer)
                .Value = item.IdElemento;

            cmd.Parameters.Add("@idBalance", NpgsqlTypes.NpgsqlDbType.Integer)
                .Value = (object?)item.IdBalance ?? DBNull.Value;

            cmd.Parameters.Add("@analisis", NpgsqlTypes.NpgsqlDbType.Boolean)
                .Value = item.Analisis;

            // 🔥 AQUÍ ESTÁ LA DIFERENCIA CLAVE
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return PlanCuentaMapper.Map(reader);
            }

            throw new Exception("No se pudo insertar la cuenta.");
        }

        public void ModificarCuenta(PlanCuenta item)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(PlanCuentaQueries.Actualizar, cn);

            cmd.Parameters.AddWithValue("@codigo", item.Codigo);
            cmd.Parameters.AddWithValue("@descripcion", item.Descripcion);
            cmd.Parameters.AddWithValue("@nivel", item.Nivel);
            cmd.Parameters.AddWithValue("@codigoPadre", (object?)item.CodigoPadre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@idElemento", item.IdElemento);
            cmd.Parameters.AddWithValue("@idBalance", (object?)item.IdBalance ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@analisis", item.Analisis);
            cmd.Parameters.AddWithValue("@idPlanCuenta", item.IdPlanCuenta);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            cmd.ExecuteNonQuery();
        }

        public void EliminarCuenta(int idPlanCuenta)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(PlanCuentaQueries.Eliminar, cn);
            cmd.Parameters.AddWithValue("@idPlanCuenta", idPlanCuenta);
            cmd.Parameters.AddWithValue("@idEmpresa", _empresaId);

            cmd.ExecuteNonQuery();
        }



        #endregion
    }
}