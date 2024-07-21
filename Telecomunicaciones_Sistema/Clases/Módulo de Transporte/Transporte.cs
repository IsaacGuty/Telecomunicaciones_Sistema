using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema.Clases.Módulo_de_Transporte
{
    public class Transporte
    {
        // Propiedad que almacena el identificador de la placa del vehículo
        public string ID_Placa { get; set; }

        // Propiedad que almacena la marca del carro
        public string Marca_Carro { get; set; }

        // Propiedad que almacena el modelo del carro
        public string Modelo_Carro { get; set; }

        // Propiedad que almacena el color del carro
        public string Color { get; set; }

        // Propiedad que almacena la fecha del último pago
        public DateTime Fecha_Pago { get; set; }

        // Propiedad que almacena el año del carro
        public string Año_Carro { get; set; }

        // Propiedad que almacena el identificador del estado del vehículo
        public string ID_Estado { get; set; }
    }
}
