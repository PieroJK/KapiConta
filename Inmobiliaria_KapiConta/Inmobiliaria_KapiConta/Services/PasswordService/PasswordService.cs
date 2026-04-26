using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Services.PasswordService
{
    public class PasswordService : IPasswordService
    {
        private const int WorkFactor = 12;
        public string HashPassword(string passwordPlainText)
        {
            if (string.IsNullOrWhiteSpace(passwordPlainText))
                throw new ArgumentException("La contraseña no puede estar vacía");
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlainText, WorkFactor);
            Debug.WriteLine($"[PasswordService] Hash generado (WorkFactor: {WorkFactor})");
            return passwordHash;
        }
        public bool VerifyPassword(string passwordPlainText, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(passwordPlainText))
                return false;

            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            return BCrypt.Net.BCrypt.Verify(passwordPlainText, storedHash);
        }
    }
}
