using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telecomunicaciones_Sistema
{
    public class InicioS
    {
        public string Usuario { get; set; }

        public string Contraseña { get; set; }

        public InicioS()
        {

        }

        public InicioS(string iUsuario, string iContraseña)
        {
            this.Usuario = iUsuario;
            this.Contraseña = iContraseña;
        }
    }
}
