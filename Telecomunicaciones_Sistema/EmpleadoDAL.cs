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
        public static DataTable ObtenerTodosEmpleados()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
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

        public static DataTable BuscarEmpleado(string criterioBusqueda)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                Conn.Open(); // Abre la conexión antes de ejecutar la consulta

                SqlCommand comando = new SqlCommand(
                    "SELECT ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado " +
                    "FROM Empleados " +
                    "WHERE ID_Empleado LIKE @Criterio OR Nombre_E LIKE @Criterio OR Apellido_E LIKE @Criterio", Conn);

                // Agrega parámetros para evitar la concatenación directa del valor del criterio de búsqueda
                comando.Parameters.AddWithValue("@Criterio", "%" + criterioBusqueda + "%");

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader); // Carga los datos directamente en el DataTable
            }
            return dataTable;
        }

        public static void ActualizarEmpleado(Empleados empleado)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
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
            using (SqlConnection connection = BD.ObtenerConexion())
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

                // Verificar si el valor de Estado es null y asignar DBNull.Value en su lugar
                object estadoValue = (object)empleado.Estado ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@Estado", estadoValue);

                cmd.ExecuteNonQuery();
            }
        }

        public static bool EmpleadoExiste(string idEmpleado)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @ID_Empleado", connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static bool EmpleadoExisteConDatos(string idEmpleado, string nombre, string apellido, string correo, string telefono, string idDireccion)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @ID_Empleado OR (Nombre_E = @Nombre_E AND Apellido_E = @Apellido_E AND Correo_E = @Correo_E AND Teléfono_E = @Teléfono_E AND ID_Dirección = @ID_Dirección)", connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                cmd.Parameters.AddWithValue("@Nombre_E", nombre);
                cmd.Parameters.AddWithValue("@Apellido_E", apellido);
                cmd.Parameters.AddWithValue("@Correo_E", correo);
                cmd.Parameters.AddWithValue("@Teléfono_E", telefono);
                cmd.Parameters.AddWithValue("@ID_Dirección", idDireccion);

                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static bool EmpleadoDI(string nombre, string apellido, string correo, string telefono, string idDireccion, string idEmpleado)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE Nombre_E = @Nombre_E AND Apellido_E = @Apellido_E AND Correo_E = @Correo_E AND Teléfono_E = @Teléfono_E AND ID_Dirección = @ID_Dirección AND ID_Empleado != @ID_Empleado", connection);
                cmd.Parameters.AddWithValue("@Nombre_E", nombre);
                cmd.Parameters.AddWithValue("@Apellido_E", apellido);
                cmd.Parameters.AddWithValue("@Correo_E", correo);
                cmd.Parameters.AddWithValue("@Teléfono_E", telefono);
                cmd.Parameters.AddWithValue("@ID_Dirección", idDireccion);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un empleado con los mismos datos, excluyendo al empleado actual
            }
        }

        public static DataTable ObtenerEmpleadosTecnicos()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    // Consulta SQL para obtener solo empleados técnicos cuyo estado es "activo"
                    string query = "SELECT * FROM Empleados WHERE Puesto = 'Tecnico' AND Estado = 'Activo'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los empleados técnicos: " + ex.Message);
            }
            return dataTable;
        }

        public static DataTable BuscarEmpleadoNombreCompleto(string nombreCompleto)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                Conn.Open();

                SqlCommand comando = new SqlCommand(
                    "SELECT * " +
                    "FROM Empleados " +
                    "WHERE CONCAT(Nombre_E, ' ', Apellido_E) = @NombreCompleto", Conn);

                comando.Parameters.AddWithValue("@NombreCompleto", nombreCompleto);

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader);
            }
            return dataTable;
        }
    }
}
