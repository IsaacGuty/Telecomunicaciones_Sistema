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

        public Pagos(string pID_Pago, string pID_Cliente, string pID_Servicio, string pServicio, decimal pMonto, string pMesPagado, DateTime pFecha, string pAño_Pagado, string pID_Empleado)
        {
            this.ID_Pago = pID_Pago;
            this.ID_Cliente = pID_Cliente;
            this.ID_Servicio = pID_Servicio;
            this.Monto = pMonto;
            this.MesPagado = pMesPagado;
            this.Fecha = pFecha;
            this.ID_Empleado = pID_Empleado;
        }
    }
}
