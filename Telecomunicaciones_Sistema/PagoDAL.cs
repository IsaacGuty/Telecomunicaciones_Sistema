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
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "SELECT P.ID_Pago, P.ID_Cliente, C.nombre, C.apellido, P.Monto, P.ID_TpServicio, P.Mes_Pagado, P.Fecha, P.ID_Empleado FROM Pagos P JOIN Cliente C ON P.ID_Cliente = C.ID_Cliente";
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

        public static DataTable BuscarPago(string textoBusqueda)
        {
            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();
                    string query = "SELECT ID_Pago, ID_Cliente, Monto, ID_TpServicio, Mes_Pagado, Fecha, ID_Empleado FROM Pagos " +
                                   "WHERE ID_Pago LIKE @TextoBusqueda";
                    SqlCommand command = new SqlCommand(query, Conn);
                    command.Parameters.AddWithValue("@TextoBusqueda", "%" + textoBusqueda + "%");

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Pago");
                    return dataSet.Tables["Pago"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los pagos del cliente: " + ex.Message);
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

                    // Verificar si el valor de MesPagado no es nulo ni está vacío antes de agregarlo como parámetro
                    if (!string.IsNullOrEmpty(pago.MesPagado))
                    {
                        cmd.Parameters.AddWithValue("@Mes_Pagado", pago.MesPagado);
                    }
                    else
                    {
                        // Manejar el caso en que el valor de MesPagado sea nulo o esté vacío
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
            // Intentar convertir textoBusqueda a entero
            int idCliente;
            bool esNumeroValido = int.TryParse(textoBusqueda, out idCliente);

            if (!esNumeroValido)
            {
                // Si textoBusqueda no es un número entero válido, puedes manejarlo como prefieras,
                throw new Exception("El texto de búsqueda debe ser un número entero válido para buscar por ID_Cliente.");
            }

            try
            {
                using (SqlConnection Conn = BD.ObtenerConexion())
                {
                    Conn.Open();

                    // Usar una búsqueda exacta en la columna ID_Cliente con el operador =
                    string query = "SELECT ID_Pago, ID_Cliente, Monto, ID_TpServicio, Mes_Pagado, Fecha, ID_Empleado FROM Pagos " +
                                   "WHERE ID_Cliente = @TextoBusqueda";

                    SqlCommand command = new SqlCommand(query, Conn);
                    // Asigna el valor entero convertido al parámetro
                    command.Parameters.AddWithValue("@TextoBusqueda", idCliente);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Pago");
                    return dataSet.Tables["Pago"];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar los pagos: " + ex.Message);
            }
        }

    }
}

