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

        public static bool CorreoRegistrado(string correo)
        {
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";
            using (SqlConnection connection = new SqlConnection(connectionString))
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
            // Cadena de conexión a la base de datos
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";

            try
            {
                // Abre la conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Inicio_Sesión SET Contraseña = @NuevaContra WHERE ID_Usuario = @Usuario";

                    // Prepara y ejecuta la consulta SQL para actualizar la contraseña
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevaContra", nuevaContra);
                        command.Parameters.AddWithValue("@Usuario", usuario);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verifica si se actualizaron filas en la base de datos
                        if (rowsAffected <= 0)
                        {
                            throw new Exception("No se encontró el usuario en la base de datos.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Error de SQL al cambiar la contraseña: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Se produjo un error inesperado al cambiar la contraseña: " + ex.Message);
            }
        }

        public static bool CamposClienteVacios(Clientes cliente)
        {
            return string.IsNullOrWhiteSpace(cliente.ID_Cliente) ||
                   string.IsNullOrWhiteSpace(cliente.Nombre) ||
                   string.IsNullOrWhiteSpace(cliente.Apellido) ||
                   string.IsNullOrWhiteSpace(cliente.Teléfono.ToString()) ||
                   string.IsNullOrWhiteSpace(cliente.Correo) ||
                   string.IsNullOrWhiteSpace(cliente.ID_Dirección);
        }

        public static bool EsTelefonoValido(string telefono)
        {
            decimal numero;
            return decimal.TryParse(telefono, out numero);
        }

        public static bool CamposEmpleadosVacios(Empleados empleado)
        {
            return string.IsNullOrWhiteSpace(empleado.ID_Empleado) ||
                   string.IsNullOrWhiteSpace(empleado.Nombre_E) ||
                   string.IsNullOrWhiteSpace(empleado.Apellido_E) ||
                   string.IsNullOrWhiteSpace(empleado.Teléfono_E.ToString()) ||
                   string.IsNullOrWhiteSpace(empleado.Correo_E) ||
                   string.IsNullOrWhiteSpace(empleado.ID_Dirección) ||
                   string.IsNullOrWhiteSpace(empleado.Puesto) ||
                   string.IsNullOrWhiteSpace(empleado.Estado);
        }
    }
}
