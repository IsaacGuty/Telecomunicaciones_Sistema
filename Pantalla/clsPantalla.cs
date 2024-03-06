using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using Datos;
using Login;

namespace Pantalla
{
    public class clsPantalla
    {
        clsDatos ObjDatos = new clsDatos();

        public DataTable Pan(clsPantalla ObjPan)
        {
            return ObjPan.Dat(ObjPan);
        }
    }
}
