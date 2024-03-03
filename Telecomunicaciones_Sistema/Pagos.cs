using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Pagos
    {
        public string ID_Cliente { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Dirección { get; set; }

        public decimal Teléfono { get; set; }

        public string Servicio { get; set; }

        public decimal Monto { get; set; }

        public string MesPagado { get; set; }

         public string Nombre_E { get; set; }

        public Pagos()
        {

        }

        public Pagos(string pID_Cliente, string pNombre, string pApellido, int pDirección, decimal pTeléfono, string pServicio, decimal pMonto, string pMesPagado, string pNombre_E)
        {
            this.ID_Cliente = pID_Cliente;
            this.Nombre = pNombre;
            this.Apellido = pApellido;
            this.Dirección = pDirección.ToString();
            this.Teléfono = pTeléfono;
            this.Servicio = pServicio;
            this.Monto = pMonto;
            this.MesPagado = pMesPagado;
            this.Nombre_E = pNombre_E;
        }
    }
}
