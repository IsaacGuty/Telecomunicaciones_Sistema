using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Telecomunicaciones_Sistema
{
    public static class InicioDAL
    {
        public static bool UsuarioExiste(string usuario)
        {
            if (!int.TryParse(usuario, out int userId))
            {
                MessageBox.Show("El usuario proporcionado no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Empleados WHERE ID_Empleado = @Usuario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", userId);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public static bool VerificarAntiguaContraseña(string usuario, string antiguaContra)
        {
            string query = "SELECT Contraseña FROM Empleados WHERE ID_Empleado = @Usuario";

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", usuario);
                    connection.Open();
                    string contraseñaAlmacenada = (string)command.ExecuteScalar();

                    return (contraseñaAlmacenada == antiguaContra);
                }
            }
        }

        public static void ActualizarContraseña(string usuario, string nuevaContra)
        {
            try
            {
                string contraseñaAlmacenada = ContraAlm(usuario);
                if (nuevaContra == contraseñaAlmacenada)
                {
                    throw new Exception("La nueva contraseña es igual a la contraseña actual. Por favor, elija una contraseña diferente.");
                }

                using (SqlConnection connection = BD.ObtenerConexion())
                {
                    string query = "UPDATE Empleados SET Contraseña = @NuevaContra WHERE ID_Empleado = @Usuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevaContra", nuevaContra);
                        command.Parameters.AddWithValue("@Usuario", usuario);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected <= 0)
                        {
                            throw new Exception("No se encontró el usuario en la base de datos.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error de SQL al cambiar la contraseña.", ex);
            }
            catch (Exception ex)
            {
                throw; 
            }
        }

        private static string ContraAlm(string usuario)
        {
            string query = "SELECT Contraseña FROM Empleados WHERE ID_Empleado = @Usuario";

            using (SqlConnection connection = BD.ObtenerConexion())
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", usuario);
                    connection.Open();
                    string contraseñaAlmacenada = (string)command.ExecuteScalar();

                    return contraseñaAlmacenada;
                }
            }
        }

        public static bool CorreoUsuario(string usuario, string correo)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM Empleados 
                    WHERE ID_Empleado = @Usuario AND Correo_E = @Correo";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", usuario);
                    command.Parameters.AddWithValue("@Correo", correo);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}
