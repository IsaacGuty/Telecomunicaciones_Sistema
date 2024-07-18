using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Telecomunicaciones_Sistema.Registro_Cliente;
using static Telecomunicaciones_Sistema.Registro_Empleado;
using static Telecomunicaciones_Sistema.Registro_Transporte;

namespace Telecomunicaciones_Sistema.Clases.Módulo_de_Transporte
{
    public static class TransporteDAL
    {
        public static DataTable ObtenerTodosTransportes()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                string query = "Select t.ID_Placa, t.Marca_Carro, t.Modelo_Carro, t.Color, t.Fecha_Pago_Matrícula, t.Año_Carro, t.ID_Estado, ea.Tipo_Estado FROM Transportes t JOIN Estado_Actividad ea ON t.ID_Estado = ea.ID_Estado   ";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            return dataTable;
        }

        public static DataTable BuscarTransporte(string textoBusqueda)
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                string query = @"
                Select t.ID_Placa, t.Marca_Carro, t.Modelo_Carro, t.Color, t.Fecha_Pago_Matrícula, t.Año_Carro, t.ID_Estado,ea.ID_Estado 
                FROM Transportes t
                JOIN Estado_Actividad ea
                ON t.ID_Estado = ea.ID_Estado
                WHERE t.ID_Placa LIKE @TextoBusqueda
                OR t.Marca_Carro LIKE @TextoBusqueda";

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

        public static bool TransporteExiste(string idPlaca)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Transportes WHERE ID_Placa = @ID_Placa", connection);
                cmd.Parameters.AddWithValue("@ID_Placa", idPlaca);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static void AgregarTransporte(Transporte transporte)
        {
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Transportes (ID_Placa, Marca_Carro, Modelo_Carro, Color, Fecha_Pago_Matrícula, Año_Carro, ID_Estado) VALUES (@ID_Placa, @Marca_Carro, @Modelo_Carro, @Color, @Fecha_Pago_Matrícula, @Año_Carro, @ID_Estado)", connection);
                    cmd.Parameters.AddWithValue("@ID_Placa", transporte.ID_Placa);
                    cmd.Parameters.AddWithValue("@Marca_Carro", transporte.Marca_Carro); 
                    cmd.Parameters.AddWithValue("@Modelo_carro", transporte.Modelo_Carro);
                    cmd.Parameters.AddWithValue("@Color", transporte.Color);
                    cmd.Parameters.AddWithValue("@Fecha_Pago_Matrícula", transporte.Fecha_Pago);
                    cmd.Parameters.AddWithValue("@Año_Carro", transporte.Año_Carro);

                    object estadoValue = (object)transporte.ID_Estado ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@ID_Estado", estadoValue);

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el transporte: " + ex.Message);
            }
        }

        public static void ActualizarTransporte(Transporte transporte)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Transportes SET Color = @Color, Fecha_Pago_Matrícula = @Fecha_Pago_Matrícula, ID_Estado = @ID_Estado", connection);
                cmd.Parameters.AddWithValue("@Color", transporte.Color);
                cmd.Parameters.AddWithValue("@Fecha_Pago_Matrícula", transporte.Fecha_Pago);

                int idEstado;
                if (!int.TryParse(transporte.ID_Estado, out idEstado))
                {
                    idEstado = 0; 
                }
                cmd.Parameters.AddWithValue("@ID_Estado", idEstado);

                cmd.ExecuteNonQuery();
            }
        }

        public static DataTable ObtenerTransportesActivos()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    connection.Open();
                    string query = "SELECT * FROM Transportes WHERE ID_Estado = '1'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los transportes activos: " + ex.Message);
            }
            return dataTable;
        }

        public static DataTable BuscarPlacaCompleto(string modelo)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                Conn.Open();

                SqlCommand comando = new SqlCommand(
                    "SELECT * " +
                    "FROM Transportes " +
                    "WHERE Modelo_Carro = @Modelo", Conn);

                comando.Parameters.AddWithValue("@Modelo", modelo);

                SqlDataReader reader = comando.ExecuteReader();

                dataTable.Load(reader);
            }
            return dataTable;
        }
    }
}
