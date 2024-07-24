using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Telecomunicaciones_Sistema 
{
    public class Datos  
    {
        // Declara una conexión SQL utilizando el método ObtenerConexion de la clase BD.
        SqlConnection Conn = BD.ObtenerConexion();

        // Define un método público que devuelve un DataTable y recibe un objeto Login como parámetro.
        public DataTable D_Users(Login objPrinc)
        {
            // Declara e inicializa un nuevo DataTable.
            DataTable DT = new DataTable();

            try  // Inicia un bloque de código que captura excepciones.
            {
                // Crea un nuevo SqlCommand para ejecutar el procedimiento almacenado "proal_login" usando la conexión Conn.
                SqlCommand cmd = new SqlCommand("proal_login", Conn);
                // Establece que el tipo de comando es un procedimiento almacenado.
                cmd.CommandType = CommandType.StoredProcedure;
                // Agrega el parámetro "@usuario" al comando con el valor del campo usuario del objeto Login.
                cmd.Parameters.AddWithValue("@usuario", objPrinc.usuario);
                // Agrega el parámetro "@contraseña" al comando con el valor del campo contraseña del objeto Login.
                cmd.Parameters.AddWithValue("@contraseña", objPrinc.contraseña);
                // Declara un SqlDataAdapter para ejecutar el comando y llenar el DataTable.
                SqlDataAdapter DA = new SqlDataAdapter(cmd);
                // Llena el DataTable con los resultados de la consulta ejecutada por el SqlDataAdapter.
                DA.Fill(DT);
            }
            catch (Exception ex)  // Captura cualquier excepción que ocurra en el bloque try.
            {
                // Escribe un mensaje de error en la consola si ocurre una excepción.
                Console.WriteLine("Error al ejecutar la consulta: " + ex.Message);
            }

            // Retorna el DataTable con los resultados de la consulta.
            return DT;
        }
    }
}

