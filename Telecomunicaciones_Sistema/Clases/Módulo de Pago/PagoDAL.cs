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
        public static DataTable ObtenerTodosPagos()
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "SELECT P.ID_Pago, P.ID_Cliente, C.nombre, C.apellido, P.Monto, P.ID_Servicio, S.Tipo_Servicio , P.Mes_Pagado, P.Fecha, P.ID_Empleado FROM Pagos P JOIN Clientes C ON P.ID_Cliente = C.ID_Cliente JOIN Servicios S on P.ID_Servicio = S.ID_Servicio";
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

        public static void AgregarPago(Pagos pago)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    SqlCommand cmd = new SqlCommand("SET IDENTITY_INSERT Pagos ON; INSERT INTO Pagos (ID_Pago, ID_Cliente, ID_Servicio, Monto, Mes_Pagado, Fecha, ID_Empleado) VALUES (@ID_Pago, @ID_Cliente, @ID_Servicio, @Monto, @Mes_Pagado, @Fecha, @ID_Empleado); SET IDENTITY_INSERT Pagos OFF;", Conn);
                    cmd.Parameters.AddWithValue("@ID_Pago", pago.ID_Pago);
                    cmd.Parameters.AddWithValue("@ID_Cliente", pago.ID_Cliente);
                    cmd.Parameters.AddWithValue("@ID_TpServicio", pago.ID_Servicio);
                    cmd.Parameters.AddWithValue("@Monto", pago.Monto);

                    if (!string.IsNullOrEmpty(pago.MesPagado))
                    {
                        cmd.Parameters.AddWithValue("@Mes_Pagado", pago.MesPagado);
                    }
                    else
                    {
                        throw new ArgumentException("El valor de MesPagado no puede ser nulo ni estar vacío.");
                    }

                    cmd.Parameters.AddWithValue("@Fecha", pago.Fecha);
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

        public static DataTable BuscarCliente(string textoBusqueda)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "SELECT P.ID_Pago, P.ID_Cliente, C.nombre, C.apellido, P.Monto, P.ID_Servicio, S.Servicio , P.Mes_Pagado, P.Fecha, P.ID_Empleado FROM Pagos P JOIN Clientes C ON P.ID_Cliente = C.ID_Cliente JOIN Servicios S on P.ID_Servicio = S.ID_Servicio WHERE P.ID_Cliente = @ID_Cliente";
                    SqlCommand cmd = new SqlCommand(query, Conn);
                    cmd.Parameters.AddWithValue("@ID_Cliente", textoBusqueda);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar pagos: " + ex.Message);
            }
        }

        public static List<Servicio> ObtenerServicioCliente(string idCliente)
        {
            List<Servicio> servicios = new List<Servicio>();
            using (SqlConnection conn = BD.ObtenerConexion())
            {
                string query = "SELECT sc.ID_Servicio, s.Tipo_Servicio " +
                               "FROM ServicioCliente sc " +
                               "JOIN Servicios s ON sc.ID_Servicio = s.ID_Servicio " +
                               "WHERE sc.ID_Cliente = @ID_Cliente " +
                               "ORDER BY CAST(sc.ID_Servicio AS INT)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID_Cliente", idCliente);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Servicio servicio = new Servicio
                    {
                        ID_Servicio = reader["ID_Servicio"].ToString(),
                        Nombre = reader["Tipo_Servicio"].ToString()
                    };
                    servicios.Add(servicio);
                }
            }
            return servicios;
        }
    }
}

