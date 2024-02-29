using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Clientes
    {
        public string ID_Cliente { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Teléfono { get; set; }

        public string Correo { get; set; }

        public string ID_Dirección { get; set; }

        public string ID_TpServicio { get; set; }

        public string MesPagado { get; set; }

        public string ID_Empleado { get; set; }

        public Clientes()
        {

        }

        public Clientes(string cID_Cliente, string cNombre, string cApellido, string cTeléfono, string cCorreo, string cID_Dirección, string cID_TpServicio, string cMesPagado, string cID_Empleado)
        {
            this.ID_Cliente = cID_Cliente;
            this.Nombre = cNombre;
            this.Apellido = cApellido;
            this.Teléfono = cTeléfono;
            this.Correo = cCorreo;
            this.ID_Dirección = cID_Dirección;
            this.ID_TpServicio = cID_TpServicio;
        }
    }
}
