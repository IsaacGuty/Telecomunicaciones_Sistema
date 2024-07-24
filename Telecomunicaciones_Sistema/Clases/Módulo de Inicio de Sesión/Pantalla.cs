using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace Telecomunicaciones_Sistema
{
    public class Pantalla  // Define la clase pública Pantalla dentro del espacio de nombres Telecomunicaciones_Sistema.
    {
        // Declara e inicializa un objeto de la clase Datos llamado objPan.
        Datos objPan = new Datos();

        // Define un método público que devuelve un DataTable y recibe un objeto Login como parámetro.
        public DataTable Pan_Users(Login objp)
        {
            // Llama al método D_Users del objeto objPan (de la clase Datos) y retorna su resultado.
            return objPan.D_Users(objp);
        }
    }
}
