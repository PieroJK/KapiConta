using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmobiliaria_KapiConta.Models.DTOs
{
    public class MesFiltroItem
    {
        public int Id { get; }
        public string Nombre { get; }

        public MesFiltroItem(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
