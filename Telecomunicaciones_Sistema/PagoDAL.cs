using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    class PagoDAL
    {
        public static List<Pagos> BuscarPago(string pID_Cliente)
        {
            List<Pagos> Lista = new List<Pagos>();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                SqlCommand comando = new SqlCommand(string.Format(
                    "select c.ID_Cliente, c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio, p.Monto, p.Mes_Pagado, e.Nombre_E from Clientes c join Pago p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio join Empleados e on e.ID_Empleado = p.ID_Empleado join Dirección d on d.ID_Dirección = c.ID_Dirección WHERE ID_Cliente LIKE '%{0}%'", pID_Cliente), Conn);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Pagos pPagos = new Pagos();
                    pPagos.ID_Cliente = reader.GetInt32(0).ToString();
                    pPagos.Nombre = reader.GetString(1);
                    pPagos.Apellido = reader.GetString(2);
                    pPagos.Dirección = reader.GetString(3);
                    pPagos.Teléfono = reader.GetDecimal(4);
                    pPagos.Servicio = reader.GetString(5);
                    pPagos.Monto = Convert.ToDecimal(reader.GetString(6));
                    pPagos.MesPagado = reader.GetString(7);
                    pPagos.Nombre_E = reader.GetString(7);

                    Lista.Add(pPagos);

                }
                Conn.Close();
                return Lista;
            }
        }
    }
}
