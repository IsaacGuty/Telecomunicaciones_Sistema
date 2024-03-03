using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Empleados
    {
        public string ID_Empleado { get; set; }

        public string Nombre_E { get; set; }

        public string Apellido_E { get; set; }

        public decimal Teléfono_E { get; set; }

        public string Correo_E { get; set; }

        public string ID_Dirección { get; set; }

        public string Puesto { get; set; }

        public string Estado { get; set; }

        public Empleados()
        {

        }

        public Empleados(string eID_Empleado, string eNombre_E, string eApellido_E, decimal eTeléfono_E, string eCorreo_E, string eID_Dirección, string ePuesto, string eEstado)
        {
            this.ID_Empleado = eID_Empleado;
            this.Nombre_E = eNombre_E;
            this.Apellido_E = eApellido_E;
            this.Teléfono_E = eTeléfono_E;
            this.Correo_E = eCorreo_E;
            this.ID_Dirección = eID_Dirección;
            this.Puesto = ePuesto;
            this.Estado = eEstado;
        }
    }
}
