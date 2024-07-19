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
                    string query = @"
                SELECT 
                    e.ID_Empleado, 
                    e.Nombre_E, 
                    e.Apellido_E, 
                    e.Teléfono_E, 
                    e.Correo_E, 
                    e.ID_Dirección, 
                    d.Dirección, 
                    e.Puesto, 
                    e.ID_Estado, 
                    ea.Tipo_Estado 
                FROM 
                    Empleados e 
                JOIN 
                    Direcciones d ON e.ID_Dirección = d.ID_Dirección 
                JOIN 
                    Estado_Actividad ea ON e.ID_Estado = ea.ID_Estado";
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
                SELECT e.ID_Empleado, e.Nombre_E, e.Apellido_E, e.Teléfono_E, e.Correo_E, e.ID_Dirección, d.Dirección, e.Puesto, e.ID_Estado
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
                string query = @"
                UPDATE Empleados 
                SET 
                Nombre_E = @Nombre_E, 
                Apellido_E = @Apellido_E, 
                Teléfono_E = @Teléfono_E, 
                Correo_E = @Correo_E, 
                ID_Dirección = @ID_Dirección, 
                Puesto = @Puesto, 
                ID_Estado = @ID_Estado 
                WHERE 
                ID_Empleado = @ID_Empleado";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@Nombre_E", empleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", empleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", empleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", empleado.Correo_E);

                int idDireccion;
                if (!int.TryParse(empleado.ID_Dirección, out idDireccion))
                {
                    idDireccion = 0; 
                }
                cmd.Parameters.AddWithValue("@ID_Dirección", idDireccion);
                cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);

                int idEstado;
                if (!int.TryParse(empleado.ID_Estado, out idEstado))
                {
                    idEstado = 0; 
                }
                cmd.Parameters.AddWithValue("@ID_Estado", idEstado);
                cmd.Parameters.AddWithValue("@ID_Empleado", empleado.ID_Empleado);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al actualizar el empleado: " + ex.Message);
                }
            }
        }

        public static void AgregarEmpleado(Empleados empleado, string contraseña)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                string query = @"
                INSERT INTO Empleados 
                (ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, ID_Estado, Contraseña) 
                VALUES 
                (@ID_Empleado, @Nombre_E, @Apellido_E, @Teléfono_E, @Correo_E, @ID_Dirección, @Puesto, @ID_Estado, @Contraseña)";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", empleado.ID_Empleado);
                cmd.Parameters.AddWithValue("@Nombre_E", empleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", empleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", empleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", empleado.Correo_E);
                cmd.Parameters.AddWithValue("@ID_Dirección", empleado.ID_Dirección);
                cmd.Parameters.AddWithValue("@Puesto", empleado.Puesto);
                cmd.Parameters.AddWithValue("@Contraseña", contraseña);

                // Ensure ID_Estado is not null
                object estadoValue = (object)empleado.ID_Estado ?? DBNull.Value;
                cmd.Parameters.AddWithValue("@ID_Estado", estadoValue);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627) // SQL Server error code for primary key violation
                    {
                        throw new Exception("El ID del empleado ya está registrado. Por favor, ingrese un ID diferente.");
                    }
                    else
                    {
                        throw new Exception("Error al agregar el empleado: " + ex.Message);
                    }
                }
            }
        }

        public static int EmpleadoExisteConDatosAg(string idEmpleado, string correo, string telefono)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();

                // Consulta SQL que verifica si hay otro empleado con los mismos datos personales
                // (correo o teléfono) y también verifica si el ID del empleado ya existe
                string query = @"
                SELECT
                CASE
                WHEN ID_Empleado = @ID_Empleado THEN 3
                WHEN Correo_E = @Correo_E THEN 1
                WHEN Teléfono_E = @Teléfono_E THEN 2
                ELSE 0
                END AS Duplicado
                FROM Empleados
                WHERE ID_Empleado = @ID_Empleado
                OR 
                Correo_E = @Correo_E 
                OR Teléfono_E = @Teléfono_E
                ;";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                cmd.Parameters.AddWithValue("@Correo_E", correo);
                cmd.Parameters.AddWithValue("@Teléfono_E", telefono);

                object result = cmd.ExecuteScalar();

                // Retornar el código de duplicado específico: 
                // 1 para correo, 2 para teléfono, 3 para ID, 0 para ninguno
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }


        public static int EmpleadoExisteConDatosMod(string idEmpleado, string correo, string telefono)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();

                // Consulta SQL que verifica si hay otro empleado con los mismos datos personales
                // (correo o teléfono) excluyendo al empleado que se está modificando por ID
                string query = @"
                SELECT
                CASE
                WHEN ID_Empleado = @ID_Empleado THEN 0  -- Excluir el propio empleado que se está modificando
                WHEN Correo_E = @Correo_E THEN 1
                WHEN Teléfono_E = @Teléfono_E THEN 2
                ELSE 0
                END AS Duplicado
                FROM Empleados
                WHERE (ID_Empleado <> @ID_Empleado)  -- Excluir el propio empleado que se está modificando
                AND (Correo_E) = @Correo_E 
                OR Teléfono_E = @Teléfono_E
                ;";

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

        public static DataTable ObtenerEmpleadosTecnicos()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    string query = "SELECT * FROM Empleados WHERE Puesto = 'Técnico' AND ID_Estado = '1'";
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
                string query = @"
                SELECT COUNT(*) 
                FROM Empleados 
                WHERE Correo_E = @Correo COLLATE Latin1_General_CS_AS";

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
