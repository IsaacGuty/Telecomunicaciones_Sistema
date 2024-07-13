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
                    string query = "select e.ID_Empleado, e.Nombre_E, e.Apellido_E, e.Teléfono_E, e.Correo_E, e.ID_Dirección, d.Dirección, e.Puesto, e.Estado from Empleados e JOIN Direcciones d ON e.ID_Dirección = d.ID_Dirección";
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
            using (SqlConnection conn = BD.ObtenerConexion())
            {
                conn.Open();
                string query = @"
                SELECT e.ID_Empleado, e.Nombre_E, e.Apellido_E, e.Teléfono_E, e.Correo_E, e.ID_Dirección, d.Dirección, e.Puesto, e.Estado
                FROM Empleados e
                JOIN Direcciones d ON e.ID_Dirección = d.ID_Dirección
                WHERE e.ID_Empleado LIKE @Criterio
                OR e.Nombre_E LIKE @Criterio
                OR e.Apellido_E LIKE @Criterio
                OR (e.Nombre_E + ' ' + e.Apellido_E) LIKE @Criterio";

                using (SqlCommand comando = new SqlCommand(query, conn))
                {
                    comando.Parameters.AddWithValue("@Criterio", "%" + criterioBusqueda + "%");

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        dataTable.Load(reader); 
                    }
                }
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

        public static void AgregarEmpleado(Empleados empleado, string contraseña)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Empleados (ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado, Contraseña) VALUES (@ID_Empleado, @Nombre_E, @Apellido_E, @Teléfono_E, @Correo_E, @ID_Dirección, @Puesto, @Estado, @Contraseña)", connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", empleado.ID_Empleado);
                cmd.Parameters.AddWithValue("@Nombre_E", empleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", empleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", empleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", empleado.Correo_E);
                cmd.Parameters.AddWithValue("@ID_Dirección", empleado.ID_Dirección);
                cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                cmd.Parameters.AddWithValue("@Contraseña", empleado.Contraseña);

                object estadoValue = (object)empleado.Estado ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@Estado", estadoValue);

                cmd.ExecuteNonQuery();
            }
        }

        /*public static bool EmpleadoExiste(string idEmpleado)
        {
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                Conn.Open();
                string query = "SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @ID_Empleado";
                using (SqlCommand cmd = new SqlCommand(query, Conn))
                {
                    cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }*/

        public static int EmpleadoExisteConDatos(string idEmpleado, string correo, string telefono)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();

                // Consulta SQL que verifica si hay otro empleado con los mismos datos personales
                // (correo o teléfono) y ID diferente al actual.
                string query = @"
                SELECT
                CASE
                     WHEN Correo_E = @Correo_E THEN 1
                     WHEN Teléfono_E = @Teléfono_E THEN 2
                ELSE 0
                END AS Duplicado
                FROM Empleados
                WHERE ID_Empleado != @ID_Empleado
                AND (
                    Correo_E = @Correo_E 
                    OR Teléfono_E = @Teléfono_E
                );
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                cmd.Parameters.AddWithValue("@Correo_E", correo);
                cmd.Parameters.AddWithValue("@Teléfono_E", telefono);

                object result = cmd.ExecuteScalar();

                // Retornar el código de duplicado específico: 
                // 1 para correo, 2 para teléfono, 0 para ninguno
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }

        /*public static bool EmpleadoDI(string correo, string telefono, string idDireccion, string idEmpleado)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Empleados WHERE Correo_E = @Correo_E AND Teléfono_E = @Teléfono_E AND ID_Dirección = @ID_Dirección AND ID_Empleado != @ID_Empleado", connection);
                cmd.Parameters.AddWithValue("@Correo_E", correo);
                cmd.Parameters.AddWithValue("@Teléfono_E", telefono);
                cmd.Parameters.AddWithValue("@ID_Dirección", idDireccion);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; 
            }
        }*/

        public static DataTable ObtenerEmpleadosTecnicos()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
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

        public static bool CorreoRegistrado(string correo)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Empleados WHERE Correo_E = @Correo COLLATE Latin1_General_CS_AS";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
