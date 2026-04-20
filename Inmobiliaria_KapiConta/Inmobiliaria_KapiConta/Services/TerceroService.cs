using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Models;
using Npgsql;
using System.Diagnostics;

namespace Inmobiliaria_KapiConta.Services
{
    public class TerceroService
    {
        public List<Tercero> Listar(string? texto = null, string? filtro = null)
        {
            var lista = new List<Tercero>();

            try
            {
                using var cn = DbConnectionFactory.Create();
                cn.Open();

                string campo = filtro switch
                {
                    "Documento" => "t.documento",
                    "Dirección" => "t.direccion",
                    _ => "t.razon_social"
                };

                string sql = string.IsNullOrWhiteSpace(texto)
                    ? TerceroQueries.Listar
                    : string.Format(TerceroQueries.ListarFiltrado, campo);

                using var cmd = new NpgsqlCommand(sql, cn);

                if (!string.IsNullOrWhiteSpace(texto))
                    cmd.Parameters.AddWithValue("@texto", $"%{texto}%");

                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                    lista.Add(TerceroMapper.Map(dr));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en Listar: " + ex);
            }

            return lista;
        }

        public Tercero? ObtenerPorDocumento(string documento)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(TerceroQueries.ObtenerPorDocumento, cn);
            cmd.Parameters.AddWithValue("@documento", documento);

            using var dr = cmd.ExecuteReader();

            return dr.Read() ? TerceroMapper.Map(dr) : null;
        }

        public bool ExisteDocumento(string documento)
        {
            try
            {
                using var cn = DbConnectionFactory.Create();
                cn.Open();

                using var cmd = new NpgsqlCommand(TerceroQueries.ExisteDocumento, cn);
                cmd.Parameters.AddWithValue("@documento", documento);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en ExisteDocumento: " + ex);
                return false;
            }
        }

        public void Insertar(Tercero t)
        {
            try
            {
                using var cn = DbConnectionFactory.Create();
                cn.Open();

                using var cmd = new NpgsqlCommand(TerceroQueries.Insertar, cn);

                cmd.Parameters.AddWithValue("@documento", t.Documento);
                cmd.Parameters.AddWithValue("@razon_social", t.RazonSocial);
                cmd.Parameters.AddWithValue("@direccion",
                    string.IsNullOrWhiteSpace(t.Direccion) ? (object)DBNull.Value : t.Direccion);
                cmd.Parameters.AddWithValue("@estado", t.Estado);
                cmd.Parameters.AddWithValue("@condicion",
                    string.IsNullOrWhiteSpace(t.Condicion) ? (object)DBNull.Value : t.Condicion);
                cmd.Parameters.AddWithValue("@tipo", t.IdTerceroTipoDocumento);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en Insertar: " + ex);
                throw;
            }
        }

        public void Actualizar(Tercero t)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(TerceroQueries.Actualizar, cn);

            cmd.Parameters.AddWithValue("@id", t.IdTercero);
            cmd.Parameters.AddWithValue("@razon_social", t.RazonSocial);
            cmd.Parameters.AddWithValue("@direccion",
                string.IsNullOrWhiteSpace(t.Direccion) ? (object)DBNull.Value : t.Direccion);
            cmd.Parameters.AddWithValue("@estado", t.Estado);
            cmd.Parameters.AddWithValue("@condicion",
                string.IsNullOrWhiteSpace(t.Condicion) ? (object)DBNull.Value : t.Condicion);
            cmd.Parameters.AddWithValue("@tipo", t.IdTerceroTipoDocumento);

            cmd.ExecuteNonQuery();
        }

        public void EliminarLogico(int idTercero)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            using var cmd = new NpgsqlCommand(TerceroQueries.EliminarLogico, cn);
            cmd.Parameters.AddWithValue("@id", idTercero);

            cmd.ExecuteNonQuery();
        }

        public List<TerceroTipoDocumento> ObtenerTiposDocumento()
        {
            var lista = new List<TerceroTipoDocumento>();

            try
            {
                using var cn = DbConnectionFactory.Create();
                cn.Open();

                using var cmd = new NpgsqlCommand(TerceroQueries.ObtenerTiposDocumento, cn);
                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                    lista.Add(TerceroMapper.MapTipo(dr));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en ObtenerTiposDocumento: " + ex);
            }

            return lista;
        }
    }
}