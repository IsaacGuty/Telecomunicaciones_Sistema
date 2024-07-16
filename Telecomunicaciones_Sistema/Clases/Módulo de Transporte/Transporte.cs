using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema.Clases.Módulo_de_Transporte
{
    public class Transporte
    {
       public string ID_Placa { get; set; }
       public string Marca_Carro { get; set; }
       public string Modelo_Carro { get; set; }
       public string Color { get; set; }
       public DateTime Fecha_Pago { get; set; }
       public string Año_Carro { get; set; }
       public string ID_Estado { get; set; }
    }
}
