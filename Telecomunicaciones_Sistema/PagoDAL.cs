using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    public static class PagoDAL
    {
        // Método para obtener todos los pagos
        public static DataTable ObtenerTodosPagos()
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true"))
                {
                    Conn.Open();
                    string query = "select c.ID_Cliente, c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio, c.Teléfono, s.Servicio, p.Monto, p.Mes_Pagado, e.Nombre_E from Cliente c join Dirección d on d.ID_Dirección = c.ID_Dirección join Pagos p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio join Empleados e on e.ID_Empleado = p.ID_Empleado";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Pago");
                    return dataSet.Tables["Pago"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los pagos: " + ex.Message);
            }
        }

        public static DataTable BuscarCliente(string clienteID)
        {
            try
            {
                using (SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true"))
                {
                    Conn.Open();
                    string query = "select c.ID_Cliente, c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio, c.Teléfono, s.Servicio, p.Monto, p.Mes_Pagado, e.Nombre_E from Cliente c join Dirección d on d.ID_Dirección = c.ID_Dirección join Pagos p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio join Empleados e on e.ID_Empleado = p.ID_Empleado WHERE c.ID_Cliente = @ClienteID";
                    SqlCommand command = new SqlCommand(query, Conn);
                    command.Parameters.AddWithValue("@ClienteID", clienteID);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "PagoCliente");
                    return dataSet.Tables["PagoCliente"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los pagos del cliente: " + ex.Message);
            }
        }

        public static void ActualizarPago(Pagos pago)
        {
            using (SqlConnection connection = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true"))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Pagos SET Mes_Pagado = @MesPagado WHERE ID_Cliente = @ID_Cliente", connection);
                cmd.Parameters.AddWithValue("@MesPagado", pago.MesPagado);
                cmd.Parameters.AddWithValue("@ID_Cliente", pago.ID_Cliente); // Utiliza el ID_Cliente actualizado
                cmd.ExecuteNonQuery();

                SqlCommand updateServiceCmd = new SqlCommand("UPDATE Servicios SET Servicio = @Servicio WHERE ID_Servicio = (SELECT ID_TpServicio FROM Pago WHERE ID_Cliente = @ID_Cliente)", connection);
                updateServiceCmd.Parameters.AddWithValue("@ID_Cliente", pago.ID_Cliente);
                updateServiceCmd.Parameters.AddWithValue("@Servicio", pago.Servicio);
                updateServiceCmd.ExecuteNonQuery();
            }
        }
    }
}
