using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace Telecomunicaciones_Sistema
{
    public class Pantalla
    {
        Datos objPan = new Datos();

        public DataTable Pan_Users (Login objp)
        {
            return objPan.D_Users(objp);
        }
    }
}
