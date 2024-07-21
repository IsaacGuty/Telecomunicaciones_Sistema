using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Clientes
    {
        // Identificador único del cliente.
        public string ID_Cliente { get; set; }

        // Nombre del cliente.
        public string Nombre { get; set; }

        // Apellido del cliente.
        public string Apellido { get; set; }

        // Número de teléfono del cliente.
        public decimal Teléfono { get; set; }

        // Dirección de correo electrónico del cliente.
        public string Correo { get; set; }

        // Identificador único de la dirección del cliente.
        public string ID_Dirección { get; set; }

        // Constructor de la clase Clientes.
        public Clientes()
        {

        }
    }
}
