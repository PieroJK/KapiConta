using Inmobiliaria_KapiConta.Data;
using Inmobiliaria_KapiConta.Data.Queries;
using Inmobiliaria_KapiConta.Data.Mappings;
using Inmobiliaria_KapiConta.Models;
using Npgsql;

namespace Inmobiliaria_KapiConta.Services
{
    public class AuthService
    {
        public Usuario Login(string usuario, string password)
        {
          using var conn = DbConnectionFactory.Create();
          conn.Open();

          using var cmd = new NpgsqlCommand(AuthQueries.LoginQuery, conn);
          cmd.Parameters.AddWithValue("@usuario", usuario);

          using var reader = cmd.ExecuteReader();

          if (!reader.Read())
          return null;

          var user = UsuarioMapper.Map(reader);

          // ?? Detectar hash inválido SIN excepción
                 if (string.IsNullOrEmpty(user.PasswordHash) || !user.PasswordHash.StartsWith("$2"))
          throw new Exception("HASH_INVALIDO");

          if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
          return null;

          return user;
        }
    }
}