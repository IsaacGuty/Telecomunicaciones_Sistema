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
    public static class PagoDAL
    {
        // Método para obtener todos los pagos
        public static DataTable ObtenerTodosPagos()
        {
            try
            {
                using(SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "select * from Pagos";
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
                using(SqlConnection Conn = BD.ObtenerConexion())
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

        public static void ModificarPago(Pagos pago)
        {
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Pagos SET ID_TpServicio = @ID_TpServicio, Monto = @Monto, Mes_Pagado = @Mes_Pagado, ID_Empleado = @ID_Empleado WHERE ID_Pago = @ID_Pago", Conn);
                cmd.Parameters.AddWithValue("@ID_TpServicio", pago.ID_TpServicio);
                cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                cmd.Parameters.AddWithValue("@Mes_Pagado", pago.MesPagado);
                cmd.Parameters.AddWithValue("@ID_Empleado", pago.ID_Empleado);
                cmd.Parameters.AddWithValue("@ID_Pago", pago.ID_Pago); // Utiliza el ID_Pago actualizado
                cmd.ExecuteNonQuery();
            }
        }

        public static void AgregarPago(Pagos pago)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Pagos ON; INSERT INTO Pagos (ID_Pago, ID_Cliente, ID_TpServicio, Monto, Mes_Pagado, Fecha, ID_Empleado) VALUES (@ID_Pago, @ID_Cliente, @ID_TpServicio, @Monto, @Mes_Pagado, @Fecha, @ID_Empleado); SET IDENTITY_INSERT Pagos OFF;", Conn);
                    cmd.Parameters.AddWithValue("@ID_Pago", pago.ID_Pago);
                    cmd.Parameters.AddWithValue("@ID_Cliente", pago.ID_Cliente);
                    cmd.Parameters.AddWithValue("@ID_TpServicio", pago.ID_TpServicio);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                    cmd.Parameters.AddWithValue("@Mes_Pagado", pago.MesPagado);
                    cmd.Parameters.AddWithValue("@Fecha", pago.Fecha); // Aquí se agrega el parámetro para la fecha
                    cmd.Parameters.AddWithValue("@ID_Empleado", pago.ID_Empleado);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al agregar el pago: " + ex.Message);
            }
        }

        public static int ObtenerUltimoIDPago()
        {
            int ultimoID = 0;
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "SELECT MAX(ID_Pago) FROM Pagos";
                    SqlCommand cmd = new SqlCommand(query, Conn);
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        ultimoID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el último ID de pago: " + ex.Message);
            }
            return ultimoID + 1;
        }
    }
}

