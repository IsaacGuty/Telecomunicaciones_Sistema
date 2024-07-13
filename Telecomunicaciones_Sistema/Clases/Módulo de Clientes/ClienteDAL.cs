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
                string query = "SELECT c.ID_Cliente, c.Nombre, c.Apellido, c.Teléfono, c.Correo, c.ID_Dirección, d.Dirección FROM Clientes C JOIN Direcciones D on c.ID_Dirección = d.ID_Dirección";
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
                string query = @"
                SELECT c.ID_Cliente, c.Nombre, c.Apellido, c.Teléfono, c.Correo, c.ID_Dirección, d.Dirección AS Dirección
                FROM Clientes c
                JOIN Direcciones d ON c.ID_Dirección = d.ID_Dirección
                WHERE c.ID_Cliente LIKE @TextoBusqueda
                OR c.Nombre LIKE @TextoBusqueda
                OR c.Apellido LIKE @TextoBusqueda
                OR (c.Nombre + ' ' + c.Apellido) LIKE @TextoBusqueda";

                using (SqlCommand comando = new SqlCommand(query, connection))
                {
                    // Agregar parámetro
                    comando.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                    using (SqlDataReader reader = comando.ExecuteReader())
                    {
                        dataTable.Load(reader); // Cargar datos en el DataTable
                    }
                }
            }

            return dataTable;
        }

       /* public static int ObtenerCantidadClientes()
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes", connection);
                return (int)cmd.ExecuteScalar();
            }
        }*/

        public static void ActualizarCliente(Clientes cliente)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Teléfono = @Teléfono, Correo = @Correo, ID_Dirección = @ID_Dirección WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);
                cmd.Parameters.AddWithValue("@ID_Cliente", cliente.ID_Cliente);
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
                    SqlCommand cmd = new SqlCommand("INSERT INTO Clientes (Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección)", connection);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                    cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                    cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                    cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);

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
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; 
            }
        }

        /*public static int ClienteDI(string idCliente, string correo, string telefono)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
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
        }*/

        public static int ObtenerUltimoIDRegistrado()
        {
            int ultimoID = 0;
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT MAX(ID_Cliente) FROM Clientes", connection);
                    object result = command.ExecuteScalar();
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

