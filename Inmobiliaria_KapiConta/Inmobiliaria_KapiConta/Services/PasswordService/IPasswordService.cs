using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Services.PasswordService
{
    public interface IPasswordService
    {
        public string HashPassword(string passwordPlainText);
        bool VerifyPassword(string plainTextPassword, string hashedPassword);
    }
}
