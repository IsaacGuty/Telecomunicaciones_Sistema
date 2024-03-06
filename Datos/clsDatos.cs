using System.Data.SqlClient;
using System.Data;
using Login;

namespace Datos
{
    public class clsDatos
    {
        SqlConnection Conn;

        public clsDatos()
        {
            Conn = BD.ObtenerConexion();
        }

        public DataTable Datos(clsLogin ObjLog)
        {
                SqlCommand cmd = new SqlCommand("proal_login", Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@usuario", ObjLog.usuario);
                cmd.Parameters.AddWithValue("@contraseña", ObjLog.usuario);
                SqlDataAdapter DA = new SqlDataAdapter(cmd);
                DataTable DT = new DataTable();
                DA.Fill(DT);
                return DT;

        }
    }
}
