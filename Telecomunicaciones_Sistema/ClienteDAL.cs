﻿using System;
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
                string query = "SELECT * FROM Cliente";
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
                    "SELECT ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección FROM Cliente WHERE ID_Cliente LIKE '%{0}%'", cID_Cliente), connection);

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
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente", connection);
                return (int)cmd.ExecuteScalar();
            }
        }

        public static void ActualizarCliente(Clientes cliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Cliente (Nombre, Apellido, Teléfono, Correo, ID_Dirección) VALUES (@Nombre, @Apellido, @Teléfono, @Correo, @ID_Dirección); SELECT SCOPE_IDENTITY();", connection);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                cmd.Parameters.AddWithValue("@Teléfono", cliente.Teléfono);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                cmd.Parameters.AddWithValue("@ID_Dirección", cliente.ID_Dirección);

                // Obtener el ID generado automáticamente
                int nuevoID = Convert.ToInt32(cmd.ExecuteScalar());

                // Asignar el nuevo ID al cliente
                cliente.ID_Cliente = nuevoID.ToString();
            }
        }

        public static bool ClienteExiste(string idCliente)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un cliente con el ID_Cliente dado
            }
        }

        public static bool ClienteDI(string idCliente, string nombre, string apellido, string correo, string telefono, string direccion)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Cliente WHERE ID_Cliente != @ID_Cliente AND Nombre = @Nombre AND Apellido = @Apellido AND Correo = @Correo AND Teléfono = @Teléfono AND ID_Dirección = @Direccion", connection);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente); // Excluir el ID del cliente actual
                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@Apellido", apellido);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Teléfono", telefono);
                cmd.Parameters.AddWithValue("@Direccion", direccion);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; // Devuelve true si se encuentra al menos un cliente con los mismos datos personales pero ID diferente
            }
        }
    }
}

