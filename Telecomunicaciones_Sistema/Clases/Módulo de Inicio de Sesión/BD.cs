using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace Telecomunicaciones_Sistema  
{
    class BD  
    {
        public static SqlConnection ObtenerConexion()
        {
            // Crea una nueva instancia de SqlConnection con una cadena de conexión específica.
            SqlConnection Conn = new SqlConnection("workstation id=TelecomunicacionesBD.mssql.somee.com;packet size=4096;user id=Mars_SQLLogin_1;pwd=54438lj1j9;data source=TelecomunicacionesBD.mssql.somee.com;persist security info=False;initial catalog=TelecomunicacionesBD;TrustServerCertificate=True");

            // Retorna la conexión SQL creada.
            return Conn;
        }
    }
}


