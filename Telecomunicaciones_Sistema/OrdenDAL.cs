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
                    string query = "SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio FROM Cliente c JOIN Dirección d ON d.ID_Dirección = c.ID_Dirección JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente JOIN Servicios s ON s.ID_Servicio = p.ID_TpServicio";
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
                    string query = "SELECT c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio FROM Cliente c JOIN Dirección d ON d.ID_Dirección = c.ID_Dirección JOIN Pagos p ON p.ID_Cliente = c.ID_Cliente JOIN Servicios s ON s.ID_Servicio = p.ID_TpServicio WHERE c.Nombre LIKE '%" + criterio + "%'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar órdenes: " + ex.Message);
            }
        }
    }
}
