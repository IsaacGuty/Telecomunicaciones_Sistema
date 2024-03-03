using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    class ClienteDAL
    {
        public static List<Clientes> BuscarCliente(string cID_Cliente)
        {
            List<Clientes> Lista = new List<Clientes>();
            using (SqlConnection Conn = BD.ObtenerConexion())
            {
                SqlCommand comando = new SqlCommand(string.Format(
                    "Select ID_Cliente, Nombre, Apellido, Teléfono, Correo, ID_Dirección from Clientes where ID_Cliente like '%{0}%'", cID_Cliente), Conn);

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Clientes cClientes = new Clientes();
                    cClientes.ID_Cliente = reader.GetInt32(0).ToString();
                    cClientes.Nombre = reader.GetString(1);
                    cClientes.Apellido = reader.GetString(2);
                    cClientes.Teléfono = reader.GetDecimal(3);
                    cClientes.Correo = reader.GetString(4);
                    cClientes.ID_Dirección = reader.GetInt32(5).ToString();

                    Lista.Add(cClientes);

                }
                Conn.Close();
                return Lista;
            }
        }
    }
}
