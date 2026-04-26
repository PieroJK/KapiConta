using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.Services.UserService
{
    public interface IUserService
    {
        void AddUser(Usuario u, string plainTextPassword);
        List<Usuario> ListUser();
        void UpdateUser(Usuario u);
        void DeleteUser(Usuario u);
    }
}
