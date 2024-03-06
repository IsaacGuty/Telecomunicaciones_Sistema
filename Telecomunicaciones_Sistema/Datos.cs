using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    public class Datos
    {
        SqlConnection Conn = BD.ObtenerConexion();

        public DataTable D_Users(Login objPrinc)
        {
            SqlCommand cmd = new SqlCommand("proal_login", Conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@usuario", objPrinc.usuario);
            cmd.Parameters.AddWithValue("@contraseña", objPrinc.contraseña);
            SqlDataAdapter DA = new SqlDataAdapter(cmd);
            DataTable DT = new DataTable();
            DA.Fill(DT);

            return DT;
        }
    }
}
