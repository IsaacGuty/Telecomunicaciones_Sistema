using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Pagos
    {
        public string ID_Pago { get; set; }

        public string ID_Cliente { get; set; }

        public decimal Monto { get; set; }

        public string ID_Servicio { get; set; }

        public string MesPagado { get; set; }

        public DateTime Fecha { get; set; }

        public string ID_Empleado { get; set; }

        public Pagos()
        {

        }
    }
}
