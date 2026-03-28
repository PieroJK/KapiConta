using Inmobiliaria_KapiConta.Data;
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
                Debug.WriteLine("🔵 Listar terceros - inicio");

                using var cn = DbConnectionFactory.Create();
                cn.Open();

                string campo = "t.razon_social";

                if (filtro == "Documento") campo = "t.documento";
                else if (filtro == "Dirección") campo = "t.direccion";

                string sql = $@"
                    SELECT 
                        t.id_tercero,
                        t.documento,
                        t.razon_social,
                        t.direccion,
                        t.estado,
                        t.condicion,
                        t.id_tercero_tipo_documento,
                        td.nombre AS tipo_documento
                    FROM tercero t
                    INNER JOIN tercero_tipo_documento td 
                        ON td.id_tercero_tipo_documento = t.id_tercero_tipo_documento
                    WHERE t.estado = true
                    {(string.IsNullOrWhiteSpace(texto) ? "" : $"AND {campo} ILIKE @texto")}
                    ORDER BY t.razon_social;";

                using var cmd = new NpgsqlCommand(sql, cn);

                if (!string.IsNullOrWhiteSpace(texto))
                    cmd.Parameters.AddWithValue("@texto", $"%{texto}%");

                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Tercero
                    {
                        IdTercero = Convert.ToInt32(dr["id_tercero"]),
                        Documento = dr["documento"]?.ToString() ?? "",
                        RazonSocial = dr["razon_social"]?.ToString() ?? "",
                        Direccion = dr["direccion"] == DBNull.Value ? null : dr["direccion"].ToString(),
                        Estado = Convert.ToBoolean(dr["estado"]),
                        Condicion = dr["condicion"] == DBNull.Value ? null : dr["condicion"].ToString(),
                        IdTerceroTipoDocumento = Convert.ToInt32(dr["id_tercero_tipo_documento"]),
                        TipoDocumentoNombre = dr["tipo_documento"]?.ToString()
                    });
                }

                Debug.WriteLine($"🟢 Listar OK - registros: {lista.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en Listar:");
                Debug.WriteLine(ex.ToString());
            }

            return lista;
        }

        public Tercero? ObtenerPorDocumento(string documento)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                SELECT *
                FROM tercero
                WHERE documento = @documento
                LIMIT 1;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@documento", documento);

            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new Tercero
                {
                    IdTercero = Convert.ToInt32(dr["id_tercero"]),
                    Documento = dr["documento"]?.ToString() ?? "",
                    RazonSocial = dr["razon_social"]?.ToString() ?? "",
                    Direccion = dr["direccion"] == DBNull.Value ? null : dr["direccion"].ToString(),
                    Estado = Convert.ToBoolean(dr["estado"]),
                    Condicion = dr["condicion"] == DBNull.Value ? null : dr["condicion"].ToString(),
                    IdTerceroTipoDocumento = Convert.ToInt32(dr["id_tercero_tipo_documento"])
                };
            }

            return null;
        }

        public bool ExisteDocumento(string documento)
        {
            try
            {
                using var cn = DbConnectionFactory.Create();
                cn.Open();

                string sql = @"SELECT COUNT(*) FROM tercero WHERE documento = @documento";

                using var cmd = new NpgsqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@documento", documento);

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en ExisteDocumento:");
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public void Insertar(Tercero t)
        {
            try
            {
                Debug.WriteLine("🔵 Insertar tercero - inicio");
                Debug.WriteLine($"Documento: {t.Documento}");
                Debug.WriteLine($"RazonSocial: {t.RazonSocial}");
                Debug.WriteLine($"Condicion: {t.Condicion}");

                using var cn = DbConnectionFactory.Create();
                cn.Open();

                string sql = @"
                    INSERT INTO tercero
                    (documento, razon_social, direccion, estado, condicion, id_tercero_tipo_documento)
                    VALUES
                    (@documento, @razon_social, @direccion, @estado, @condicion, @tipo);";

                using var cmd = new NpgsqlCommand(sql, cn);

                cmd.Parameters.AddWithValue("@documento", t.Documento);
                cmd.Parameters.AddWithValue("@razon_social", t.RazonSocial);
                cmd.Parameters.AddWithValue("@direccion",
                    string.IsNullOrWhiteSpace(t.Direccion) ? (object)DBNull.Value : t.Direccion);
                cmd.Parameters.AddWithValue("@estado", t.Estado);
                cmd.Parameters.AddWithValue("@condicion",
                    string.IsNullOrWhiteSpace(t.Condicion) ? (object)DBNull.Value : t.Condicion);
                cmd.Parameters.AddWithValue("@tipo", t.IdTerceroTipoDocumento);

                cmd.ExecuteNonQuery();

                Debug.WriteLine("🟢 Insertar OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR en Insertar:");
                Debug.WriteLine(ex.ToString());

                throw; // 🔥 IMPORTANTE: vuelve a lanzar para que lo capture el ViewModel
            }
        }

        public void Actualizar(Tercero t)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                UPDATE tercero
                SET razon_social = @razon_social,
                    direccion = @direccion,
                    estado = @estado,
                    condicion = @condicion,
                    id_tercero_tipo_documento = @tipo
                WHERE id_tercero = @id;";

            using var cmd = new NpgsqlCommand(sql, cn);

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

        // 🔹 OPCIONAL PERO MUY RECOMENDADO
        public void EliminarLogico(int idTercero)
        {
            using var cn = DbConnectionFactory.Create();
            cn.Open();

            string sql = @"
                UPDATE tercero
                SET estado = false
                WHERE id_tercero = @id;";

            using var cmd = new NpgsqlCommand(sql, cn);
            cmd.Parameters.AddWithValue("@id", idTercero);

            cmd.ExecuteNonQuery();
        }

        public List<TerceroTipoDocumento> ObtenerTiposDocumento()
        {
            var lista = new List<TerceroTipoDocumento>();

            try
            {
                Debug.WriteLine("🔵 Conectando a BD...");

                using var cn = DbConnectionFactory.Create();
                cn.Open();

                Debug.WriteLine("🟢 Conexión OK");

                string sql = @"
        SELECT id_tercero_tipo_documento, cod, nombre
        FROM tercero_tipo_documento
        ORDER BY cod;";

                using var cmd = new NpgsqlCommand(sql, cn);
                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new TerceroTipoDocumento
                    {
                        IdTerceroTipoDocumento = Convert.ToInt32(dr["id_tercero_tipo_documento"]),
                        Cod = Convert.ToInt32(dr["cod"]),
                        Nombre = dr["nombre"]?.ToString()
                    });
                }

                Debug.WriteLine($"🟢 Registros cargados: {lista.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("🔴 ERROR EN BD:");
                Debug.WriteLine(ex.ToString());
            }

            return lista;
        }
    }
}