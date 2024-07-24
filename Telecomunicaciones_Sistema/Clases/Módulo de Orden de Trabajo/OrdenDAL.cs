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
                    string query = @"
                    SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Tipo_Servicio
                    FROM Clientes c
                    JOIN Direcciones d ON d.ID_Dirección = c.ID_Dirección
                    JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente
                    JOIN Servicios s ON s.ID_Servicio = p.ID_Servicio
                    WHERE p.ID_Cliente = c.ID_Cliente
                    GROUP BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Tipo_Servicio
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

                    string query = @"
                    SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Tipo_Servicio
                    FROM Clientes c
                    JOIN Direcciones d ON d.ID_Dirección = c.ID_Dirección
                    JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente
                    JOIN Servicios s ON s.ID_Servicio = p.ID_Servicio
                    WHERE c.Nombre LIKE @criterio
                    OR c.Apellido LIKE @criterio
                    OR (c.Nombre + ' ' + c.Apellido) LIKE @criterio
                    GROUP BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Tipo_Servicio
                    ORDER BY c.Nombre, c.Apellido, d.Dirección, c.Teléfono;
                    ";

                    using (SqlCommand cmd = new SqlCommand(query, Conn))
                    {
                        cmd.Parameters.AddWithValue("@criterio", "%" + criterio + "%");

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
                    string query = "INSERT INTO Ordenes (Nombre, Apellido, Dirección, Teléfono, Servicio, Tipo_Trabajo, Nombre_E, ID_Empleado, Fecha_Orden, ID_Placa, Modelo_Carro) VALUES (@Nombre, @Apellido, @Dirección, @Teléfono, @Servicio, @Tipo_Trabajo, @Nombre_E, @ID_Empleado, @Fecha_Orden, @ID_Placa, @Modelo_Carro)";
                    SqlCommand command = new SqlCommand(query, Conn);
                    command.Parameters.AddWithValue("@Nombre", orden.Nombre);
                    command.Parameters.AddWithValue("@Apellido", orden.Apellido);
                    command.Parameters.AddWithValue("@Dirección", orden.Dirección);
                    command.Parameters.AddWithValue("@Teléfono", orden.Teléfono);
                    command.Parameters.AddWithValue("@Servicio", orden.Servicio);
                    command.Parameters.AddWithValue("@Tipo_Trabajo", orden.Tipo_Trabajo);
                    command.Parameters.AddWithValue("@Nombre_E", orden.Nombre_E);
                    command.Parameters.AddWithValue("@ID_Empleado", orden.ID_Empleado);
                    command.Parameters.AddWithValue("@Fecha_Orden", orden.Fecha_Orden);
                    command.Parameters.AddWithValue("@ID_Placa", orden.ID_Placa);
                    command.Parameters.AddWithValue("@Modelo_Carro", orden.Modelo_Carro);

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
