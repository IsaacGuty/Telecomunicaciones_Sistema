using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    class EmpleadoDAL
    {
        public static List<Empleados> BuscarEmpleado(string eID_Empleado)
        {
            List<Empleados> Lista = new List<Empleados>();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                SqlCommand comando = new SqlCommand(string.Format(
                    "SELECT ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado FROM Empleados WHERE ID_Empleado LIKE '%{0}%'", eID_Empleado), Conn);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Empleados eEmpleados = new Empleados();
                    eEmpleados.ID_Empleado = reader.GetInt32(0).ToString();
                    eEmpleados.Nombre_E = reader.GetString(1);
                    eEmpleados.Apellido_E = reader.GetString(2);
                    eEmpleados.Teléfono_E = reader.GetDecimal(3);
                    eEmpleados.Correo_E = reader.GetString(4);
                    eEmpleados.ID_Dirección = reader.GetInt32(5).ToString();
                    eEmpleados.Puesto = reader.GetString(6);
                    eEmpleados.Estado = reader.GetString(7);

                    Lista.Add(eEmpleados);

                }
                Conn.Close();
                return Lista;
            }
        }
    }
}
