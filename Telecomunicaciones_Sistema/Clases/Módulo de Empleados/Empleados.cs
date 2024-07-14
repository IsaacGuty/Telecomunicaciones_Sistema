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
        public string ID_Estado { get; set; }
        public string Contraseña { get; set; } 

        public Empleados()
        {
        }
    }
}
