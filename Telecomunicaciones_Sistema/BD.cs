using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace Telecomunicaciones_Sistema
{
    class BD
    {
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection Conn = new SqlConnection("Data source = LAPTOP-GTMSPJHL\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            // Conn.Open();

            return Conn;
        }
    }
}

