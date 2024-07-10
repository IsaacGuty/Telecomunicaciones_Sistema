using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Servicio
    {
        public string ID_Servicio { get; set; }
        public string Nombre { get; set; }

        public Servicio()
        {

        }

        public Servicio(string sID_Servicio, string sNombre)
        {
            this.ID_Servicio = sID_Servicio;
            this.Nombre = sNombre;
        }
    }
}
