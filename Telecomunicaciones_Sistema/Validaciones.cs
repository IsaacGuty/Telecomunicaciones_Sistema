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

            using (SqlConnection connection = BD.ObtenerConexion())
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
            string query = "SELECT Contraseña FROM Inicio_Sesión WHERE ID_Usuario = @Usuario";

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
                // Obtener la contraseña almacenada en la base de datos para el usuario dado
                string contraseñaAlmacenada = ContraAlm(usuario);

                // Verificar si la nueva contraseña es diferente de la contraseña almacenada
                if (nuevaContra == contraseñaAlmacenada)
                {
                    throw new Exception("La nueva contraseña es igual a la contraseña actual. Por favor, elija una contraseña diferente.");
                }

                // Si la nueva contraseña es diferente, proceder con la actualización en la base de datos
                using (SqlConnection connection = BD.ObtenerConexion())
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
                throw; // Dejar que la excepción sea relanzada tal como está, sin agregar mensaje adicional
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

        public static bool CorreoRegistrado(string correo)
        {
            using (SqlConnection connection = BD.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Empleados WHERE Correo_E = @Correo";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Correo", correo);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public static void ActualizarContraseñaa(string usuario, string nuevaContra)
        {
            try
            {
                // Obtener la contraseña almacenada en la base de datos para el usuario dado
                string contraseñaAlmacenada = ContraAlm(usuario);

                // Verificar si la nueva contraseña es diferente de la contraseña almacenada
                if (nuevaContra == contraseñaAlmacenada)
                {
                    throw new Exception("La nueva contraseña es igual a la contraseña actual. Por favor, elija una contraseña diferente.");
                }

                // Si la nueva contraseña es diferente, proceder con la actualización en la base de datos
                using (SqlConnection connection = BD.ObtenerConexion())
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
                throw new Exception("Se produjo un error al cambiar la contraseña: " + ex.Message);
            }
        }

        private static string ContraAlm(string usuario)
        {
            string query = "SELECT Contraseña FROM Inicio_Sesión WHERE ID_Usuario = @Usuario";

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

        public static bool CamposClienteVacios(string nombre, string apellido, string telefono, string correo, string direccion)
        {
            return string.IsNullOrWhiteSpace(nombre) ||
                   string.IsNullOrWhiteSpace(apellido) ||
                   string.IsNullOrWhiteSpace(telefono) ||
                   string.IsNullOrWhiteSpace(correo) ||
                   string.IsNullOrWhiteSpace(direccion);
        }


        public static bool EsTelefonoValido(string telefono)
        {
            // Verificar si el teléfono tiene 8 dígitos y comienza con 3, 8 o 9
            return telefono.Length == 8 && (telefono.StartsWith("3") || telefono.StartsWith("8") || telefono.StartsWith("9"));
        }

        public static bool CamposEmpleadosVacios(string idEmpleado, string nombre, string apellido, string telefono, string correo, string direccion)
        {
            return string.IsNullOrWhiteSpace(idEmpleado) ||
                   string.IsNullOrWhiteSpace(nombre) ||
                   string.IsNullOrWhiteSpace(apellido) ||
                   string.IsNullOrWhiteSpace(telefono) ||
                   string.IsNullOrWhiteSpace(correo) ||
                   string.IsNullOrWhiteSpace(direccion);
        }
    }
}
