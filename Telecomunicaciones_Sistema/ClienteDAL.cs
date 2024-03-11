using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    public static class ClienteDAL
    {
        private static string connectionString = "Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true";

        public static DataTable ObtenerTodosClientes()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Clientes";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            return dataTable;
        }

        public static DataTable BuscarCliente(string cID_Cliente)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand comando = new SqlCommand(string.Format(
                    "SELECT ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección FROM Clientes WHERE ID_Cliente LIKE '%{0}%'", cID_Cliente), connection);

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader); // Carga los datos directamente en el DataTable
            }
            return dataTable;
        }

        public static int ObtenerCantidadClientes()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes", connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static void ActualizarCliente(Clientes cliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Teléfono = @Teléfono, Correo = @Correo, ID_Dirección = @ID_Dirección WHERE ID_Cliente = @ID_Cliente", connection);
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Clientes (ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@ID_Cliente, @Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección)", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", cliente.ID_Cliente);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);
                cmd.ExecuteNonQuery();
            }
        }

        public static bool ClienteExiste(string idCliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Clientes WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un cliente con el ID_Cliente dado
            }
        }

    }
}

