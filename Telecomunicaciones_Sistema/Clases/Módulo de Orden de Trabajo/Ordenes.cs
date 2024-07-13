using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Ordenes
    {
        public string ID_Orden { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Dirección { get; set; }

        public decimal Teléfono { get; set; }

        public string Servicio { get; set; }

        public string Tp_Servicio { get; set; }

        public string ID_Empleado { get; set; }

        public string Nombre_E { get; set; }

        public Ordenes()
        {

        }

        public Ordenes(string oNombre, string oApellido, string oDirección, decimal oTeléfono, string oServicio, string oTp_Servicio, string oID_Empleado, string oNombre_E)
        {
            this.Nombre = oNombre;
            this.Apellido = oApellido;
            this.Dirección = oDirección;
            this.Teléfono = oTeléfono;
            this.Servicio = oServicio;
            this.Tp_Servicio = oTp_Servicio;
            this.ID_Empleado = oID_Empleado;
            this.Nombre_E = oNombre_E;
        }
    }
}
