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

        public string Tipo_Trabajo { get; set; }

        public string ID_Empleado { get; set; }

        public string Nombre_E { get; set; }

        public string ID_Placa { get; set; }

        public string Modelo_Carro { get; set; }

        public DateTime Fecha_Orden { get; set; }

        public Ordenes()
        {

        }
    }
}
