using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Pagos
    {
        // Identificador único del pago
        public string ID_Pago { get; set; }

        // Identificador del cliente asociado al pago
        public string ID_Cliente { get; set; }

        // Monto del pago realizado
        public decimal Monto { get; set; }

        // Identificador del servicio asociado al pago
        public string ID_Servicio { get; set; }

        // Mes al que corresponde el pago
        public string MesPagado { get; set; }

        // Fecha en la que se realizó el pago
        public DateTime Fecha { get; set; }

        // Identificador del empleado que registró el pago
        public string ID_Empleado { get; set; }

        // Constructor por defecto de la clase Pagos
        public Pagos()
        {

        }
    }
}
