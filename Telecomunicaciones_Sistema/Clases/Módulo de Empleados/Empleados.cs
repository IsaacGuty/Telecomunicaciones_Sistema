using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    // Representa un empleado en el sistema.
    public class Empleados
    {
        // Identificador único del empleado.
        public string ID_Empleado { get; set; }

        // Nombre del empleado.
        public string Nombre_E { get; set; }

        // Apellido del empleado.
        public string Apellido_E { get; set; }

        // Número de teléfono del empleado.
        public decimal Teléfono_E { get; set; }

        // Correo electrónico del empleado.
        public string Correo_E { get; set; }

        // Identificador de la dirección del empleado.
        public string ID_Dirección { get; set; }

        // Puesto del empleado en la empresa.
        public string Puesto { get; set; }

        // Identificador del estado del empleado (activo, inactivo, etc.).
        public string ID_Estado { get; set; }

        // Contraseña del empleado.
        public string Contraseña { get; set; }

        // Inicializa una nueva instancia de la clase Empleados.
        public Empleados()
        {
        }
    }
}
