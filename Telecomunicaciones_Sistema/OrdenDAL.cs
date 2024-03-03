using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    class OrdenDAL
    {
        public static List<Ordenes> BuscarOrden(string Nombre)
        {
            List<Ordenes> Lista = new List<Ordenes>();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                SqlCommand comando = new SqlCommand(string.Format(
                    "select c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio from Clientes c join Dirección d on d.ID_Dirección = c.ID_Dirección join Pago p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio WHERE c.Nombre LIKE '%{0}%'", Nombre), Conn);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Ordenes oOrdenes = new Ordenes();
                    oOrdenes.Nombre = reader.GetString(0);
                    oOrdenes.Apellido = reader.GetString(1);
                    oOrdenes.Dirección = reader.GetString(2);
                    oOrdenes.Teléfono = reader.GetDecimal(3);
                    oOrdenes.Servicio = reader.GetString(4);

                    Lista.Add(oOrdenes);

                }
                Conn.Close();
                return Lista;
            }
        }
    }
}
