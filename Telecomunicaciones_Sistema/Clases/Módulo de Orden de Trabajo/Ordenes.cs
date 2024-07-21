using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class Ordenes
    {
        // Propiedad para almacenar el identificador único de la orden
        public string ID_Orden { get; set; }

        // Propiedad para almacenar el nombre del cliente
        public string Nombre { get; set; }

        // Propiedad para almacenar el apellido del cliente
        public string Apellido { get; set; }

        // Propiedad para almacenar la dirección del cliente
        public string Dirección { get; set; }

        // Propiedad para almacenar el número de teléfono del cliente
        public decimal Teléfono { get; set; }

        // Propiedad para almacenar el tipo de servicio solicitado
        public string Servicio { get; set; }

        // Propiedad para almacenar el tipo de trabajo a realizar
        public string Tipo_Trabajo { get; set; }

        // Propiedad para almacenar el identificador del empleado asignado
        public string ID_Empleado { get; set; }

        // Propiedad para almacenar el nombre del empleado asignado
        public string Nombre_E { get; set; }

        // Propiedad para almacenar el identificador de la placa del vehículo
        public string ID_Placa { get; set; }

        // Propiedad para almacenar el modelo del vehículo
        public string Modelo_Carro { get; set; }

        // Propiedad para almacenar la fecha en que se creó la orden
        public DateTime Fecha_Orden { get; set; }

        // Constructor de la clase Ordenes
        public Ordenes()
        {

        }
    }
}
