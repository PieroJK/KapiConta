using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inmobiliaria_KapiConta.Models;

namespace Inmobiliaria_KapiConta.Services
{
    public interface IEnterpriseService
    {
        void AddEnterprise(Empresa e);
        List<Empresa> ListEnterprise();
    }
}
