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
    public static class OrdenDAL
    {
        public static DataTable ObtenerOrdenes()
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();

                    // Consulta SQL que agrupa por cliente y dirección, selecciona los detalles del cliente y muestra múltiples filas si el cliente tiene servicios distintos
                    string query = @"
                    SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio
                    FROM Cliente c
                    JOIN Dirección d ON d.ID_Dirección = c.ID_Dirección
                    JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente
                    JOIN Servicios s ON s.ID_Servicio = p.ID_TpServicio
                    WHERE p.ID_Cliente = c.ID_Cliente
                    GROUP BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio
                    ORDER BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono;
            ";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las órdenes: " + ex.Message);
            }
        }

        public static DataTable BuscarOrden(string criterio)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();

                    // Consulta SQL que agrupa por cliente y dirección, selecciona los detalles del cliente y muestra múltiples filas si el cliente tiene servicios distintos
                    string query = @"
                    SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio
                    FROM Cliente c
                    JOIN Dirección d ON d.ID_Dirección = c.ID_Dirección
                    JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente
                    JOIN Servicios s ON s.ID_Servicio = p.ID_TpServicio
                    WHERE c.Nombre LIKE @criterio
                    GROUP BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio
                    ORDER BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono;
            ";

                    // Usa SqlCommand y parámetros para prevenir la inyección de SQL
                    using (SqlCommand cmd = new SqlCommand(query, Conn))
                    {
                        // Agregar parámetro @criterio a la consulta
                        cmd.Parameters.AddWithValue("@criterio", "%" + criterio + "%");

                        // Llena el DataTable utilizando un SqlDataAdapter
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar órdenes: " + ex.Message);
            }
        }

        public static void GuardarOrden(Ordenes orden)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "INSERT INTO Ordenes (Nombre, Apellido, Dirección, Teléfono, Servicio, Tp_Servicio, Nombre_E, ID_Empleado) VALUES (@Nombre, @Apellido, @Dirección, @Teléfono, @Servicio, @Tp_Servicio, @Nombre_E, @ID_Empleado)";
                    SqlCommand command = new SqlCommand(query, Conn);
                    command.Parameters.AddWithValue("@Nombre", orden.Nombre);
                    command.Parameters.AddWithValue("@Apellido", orden.Apellido);
                    command.Parameters.AddWithValue("@Dirección", orden.Dirección);
                    command.Parameters.AddWithValue("@Teléfono", orden.Teléfono);
                    command.Parameters.AddWithValue("@Servicio", orden.Servicio);
                    command.Parameters.AddWithValue("@Tp_Servicio", orden.Tp_Servicio);
                    command.Parameters.AddWithValue("@Nombre_E", orden.Nombre_E);
                    command.Parameters.AddWithValue("@ID_Empleado", orden.ID_Empleado);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la orden: " + ex.Message);
            }
        }

    }
}
