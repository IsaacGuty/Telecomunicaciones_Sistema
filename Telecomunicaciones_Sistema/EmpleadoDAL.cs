using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    public static class EmpleadoDAL
    {
        private static string connectionString = "Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true";

        public static DataTable ObtenerTodosEmpleados()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Empleados";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los empleados: " + ex.Message);
            }
            return dataTable;
        }

        public static DataTable BuscarEmpleado(string eID_Empleado)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                SqlCommand comando = new SqlCommand(string.Format(
                    "SELECT ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado FROM Empleados WHERE ID_Empleado LIKE '%{0}%'", eID_Empleado), Conn);

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader); // Carga los datos directamente en el DataTable

                Conn.Close();
            }
            return dataTable;
        }

        public static void ActualizarEmpleado(Empleados empleado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Empleados SET Nombre_E = @Nombre_E, Apellido_E = @Apellido_E, Teléfono_E = @Teléfono_E, Correo_E = @Correo_E, ID_Dirección = @ID_Dirección, Puesto = @Puesto, Estado = @Estado WHERE ID_Empleado = @ID_Empleado", connection);
                cmd.Parameters.AddWithValue("@Nombre_E", empleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", empleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", empleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", empleado.Correo_E);
                cmd.Parameters.AddWithValue("@ID_Dirección", empleado.ID_Dirección);
                cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                cmd.Parameters.AddWithValue("@Estado", empleado.Estado);
                cmd.Parameters.AddWithValue("@ID_Empleado", empleado.ID_Empleado);
                cmd.ExecuteNonQuery();
            }
        }

        public static void AgregarEmpleado(Empleados empleado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Empleados (ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado) VALUES (@ID_Empleado, @Nombre_E, @Apellido_E, @Teléfono_E, @Correo_E, @ID_Dirección, @Puesto, @Estado)", connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", empleado.ID_Empleado);
                cmd.Parameters.AddWithValue("@Nombre_E", empleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", empleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", empleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", empleado.Correo_E);
                cmd.Parameters.AddWithValue("@ID_Dirección", empleado.ID_Dirección);
                cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                cmd.Parameters.AddWithValue("@Estado", empleado.Estado);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool EmpleadoExiste(string idEmpleado)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @ID_Empleado", connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un empleado con el ID_Empleado dado
            }
        }

    }
}
