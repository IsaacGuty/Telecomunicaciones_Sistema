using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows;

namespace Telecomunicaciones_Sistema
{
    public static class ClienteDAL
    {
        public static DataTable ObtenerTodosClientes()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                string query = "SELECT * FROM Cliente";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            return dataTable;
        }

        public static DataTable BuscarCliente(string textoBusqueda)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                // Modificamos la consulta SQL para buscar por ID_Cliente, Nombre o Apellido
                SqlCommand comando = new SqlCommand(string.Format(
                    "SELECT ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección FROM Cliente " +
                    "WHERE ID_Cliente LIKE '%{0}%' OR Nombre LIKE '%{0}%' OR Apellido LIKE '%{0}%'" +
                    "OR (Nombre + ' ' + Apellido) LIKE '%{0}%'", textoBusqueda), connection);

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader); // Carga los datos directamente en el DataTable
            }
            return dataTable;
        }

        public static int ObtenerCantidadClientes()
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente", connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static void ActualizarCliente(Clientes cliente)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Cliente SET Nombre = @Nombre, Apellido = @Apellido, Teléfono = @Teléfono, Correo = @Correo, ID_Dirección = @ID_Dirección WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);
                cmd.Parameters.AddWithValue("@ID_Cliente", cliente.ID_Cliente); // Utiliza el ID_Cliente actualizado
                cmd.ExecuteNonQuery();
            }
        }

        public static void AgregarCliente(Clientes cliente)
        {
            try
            {   
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Cliente (Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección)", connection);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                    cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                    cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                    cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);

                    // Ejecutar el comando de inserción
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el cliente: " + ex.Message);
            }
        }

        public static bool ClienteExiste(string idCliente)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un cliente con el ID_Cliente dado
            }
        }

        public static int ClienteDI(string idCliente, string correo, string telefono)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();

                // Consulta SQL que verifica si hay otro cliente con los mismos datos personales
                // (correo o teléfono) y ID diferente al actual.
                string query = @"
                SELECT
                CASE
                     WHEN Correo = @Correo THEN 1
                     WHEN Teléfono = @Teléfono THEN 2
                ELSE 0
                END AS Duplicado
                FROM Cliente
                     WHERE ID_Cliente != @ID_Cliente
                AND (
                    Correo = @Correo 
                OR Teléfono = @Teléfono
                );
                ";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Teléfono", telefono);

                object result = cmd.ExecuteScalar();

                // Retornar el código de duplicado específico: 
                // 1 para correo, 2 para teléfono, 0 para ninguno
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
        }


        public static int ObtenerUltimoIDRegistrado()
        {
            int ultimoID = 0;
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    // Abre la conexión
                    connection.Open();

                    // Crea un comando SQL para obtener el último ID registrado
                    SqlCommand command = new SqlCommand("SELECT MAX(ID_Cliente) FROM Cliente", connection);

                    // Ejecuta el comando y obtén el resultado
                    object result = command.ExecuteScalar();

                    // Verifica si el resultado no es nulo y si es un número entero
                    if (result != DBNull.Value && int.TryParse(result.ToString(), out int id))
                    {
                        ultimoID = id;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el último ID registrado: " + ex.Message);
            }
            return ultimoID;
        }
    }
}

