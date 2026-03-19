using Npgsql;
using Inmobiliaria_KapiConta.Data;

public class AuthService
{
    public bool Login(string usuario, string password)
    {
        using var conn = DbConnectionFactory.Create();
        conn.Open();

        string query = @"SELECT clave_hash 
                         FROM usuario 
                         WHERE usuario = @usuario AND estado = TRUE";

        using var cmd = new NpgsqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@usuario", usuario);

        var result = cmd.ExecuteScalar();

        if (result == null)
            return false;

        string hash = result.ToString();

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}