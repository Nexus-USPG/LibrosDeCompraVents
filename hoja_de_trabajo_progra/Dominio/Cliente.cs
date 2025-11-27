using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hoja_de_trabajo_progra.Dominio
{
    public class Cliente
    {
        public int Id { get; set; } = Guid.NewGuid().GetHashCode();
        public string NIT { get; set; } = "C/F";
        public string Nombre { get; set; }

        public string Telefono { get; set; }
        public Cliente(int id, string nit, string nombre, string telefono)
        {
            Id = id;
            NIT = nit;
            Nombre = nombre;
            Telefono = telefono;
        }
    }
}
