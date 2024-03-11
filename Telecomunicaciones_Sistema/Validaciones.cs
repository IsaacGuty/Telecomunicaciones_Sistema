using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Windows;

namespace Telecomunicaciones_Sistema
{
    class Validaciones
    {
        public static bool CorreoValido(string correo)
        {
            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(correo, patron);
        }

        public static bool UsuarioExiste(string usuario)
        {
            if (!int.TryParse(usuario, out int userId))
            {
                MessageBox.Show("El usuario proporcionado no es válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Inicio_Sesión WHERE ID_Usuario = @Usuario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", userId);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public static bool CamposContraseñaCompletos(string anteriorContra, string nuevaContra, string confirmarContra)
        {
            return !(string.IsNullOrEmpty(anteriorContra) || string.IsNullOrEmpty(nuevaContra) || string.IsNullOrEmpty(confirmarContra));
        }

        public static bool ContraseñasCoinciden(string nuevaContra, string confirmarContra)
        {
            return nuevaContra == confirmarContra;
        }

        public static bool VerificarAntiguaContraseña(string usuario, string antiguaContra)
        {
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";
            string query = "SELECT Contraseña FROM Inicio_Sesión WHERE ID_Usuario = @Usuario";

            using (SqlConnection connection = new SqlConnection(connectionString))
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
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Inicio_Sesión SET Contraseña = @NuevaContra WHERE ID_Usuario = @Usuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevaContra", nuevaContra);
                        command.Parameters.AddWithValue("@Usuario", usuario);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected <= 0)
                        {
                            MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error de SQL al cambiar la contraseña.", ex);
            }
        }

        public static bool CamposContraseñaVacios(string nuevaContra, string confirmarContra)
        {
            return string.IsNullOrEmpty(nuevaContra) || string.IsNullOrEmpty(confirmarContra);
        }

        public static bool ContraseñasNoCoinciden(string nuevaContra, string confirmarContra)
        {
            return nuevaContra != confirmarContra;
        }

        public static bool CamposCorreoUsuarioVacios(string usuario, string correo)
        {
            return string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(correo);
        }

        public static bool IsGerenteGeneral(string rol)
        {
            return rol == "Gerente General";
        }

        public static bool IsGerenteTecnico(string rol)
        {
            return rol == "Gerente Tecnico";
        }

        public static bool IsTecnico(string rol)
        {
            return rol == "Tecnico";
        }

        public static bool IsSecretaria(string rol)
        {
            return rol == "Secretaria" || rol == "Secretario";
        }

        public static bool IsContadora(string rol)
        {
            return rol == "Contadora" || rol == "Contador";
        }

    }
}
