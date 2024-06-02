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
using System.Windows.Controls;
using System.Printing;
using System.Globalization;
using System.Windows.Input;

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

        public static bool CamposVacios(string usuario, string contraseña)
        {
            return string.IsNullOrWhiteSpace(usuario) && string.IsNullOrWhiteSpace(contraseña);
        }

        public static bool UsuarioVacio(string usuario)
        {
            return string.IsNullOrWhiteSpace(usuario);
        }

        public static bool ContraseñaVacia(string contraseña)
        {
            return string.IsNullOrWhiteSpace(contraseña);
        }

        public static bool ContieneSoloNumeros(string texto)
        {
            foreach (char c in texto)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ValidarUsuarioYContraseña(string usuario, string contraseña)
        {
            return string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contraseña);
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

        public static bool CamposClienteVacios(string nombre, string apellido, string telefono, string correo, string direccion, ComboBox cmbDire)
        {
            return string.IsNullOrWhiteSpace(nombre) ||
                   string.IsNullOrWhiteSpace(apellido) ||
                   string.IsNullOrWhiteSpace(telefono) ||
                   string.IsNullOrWhiteSpace(correo) ||
                   string.IsNullOrWhiteSpace(direccion) ||
                   cmbDire.SelectedItem == null;
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

        public static bool CamposPagoVacios(string idPago, string idCliente, string nombreEmpleado, ComboBox cmbMes, string monto)
        {
            return !string.IsNullOrEmpty(idPago) &&
                   !string.IsNullOrEmpty(idCliente) &&
                   !string.IsNullOrEmpty(nombreEmpleado) &&
                   cmbMes.SelectedItem != null &&
                   !string.IsNullOrEmpty(monto);
        }

        public static bool CamposOrdenVacios(string nombre, string apellido, string direccion, string telefono, string servicio, object tipoT, object nombreE)
        {
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(direccion) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(servicio) || tipoT == null || nombreE == null)
            {
                return false;
            }
            return true;
        }

        public static bool ContieneEspaciosEnBlanco(string cadena)
        {
            return string.IsNullOrWhiteSpace(cadena);
        }

        public static bool NoContieneEspaciosEnBlanco(string texto)
        {
            return !string.IsNullOrWhiteSpace(texto) && texto.Trim().Length > 0;
        }

        public static bool NombreValido(string nombre)
        {
            // Expresión regular para verificar que el nombre contenga solo dos palabras separadas por un espacio
            Regex regex = new Regex(@"^\p{L}+(?: \p{L}+)?$");

            // Verifica si el nombre coincide con la expresión regular
            return regex.IsMatch(nombre);
        }

        public static bool TresVecesSeguidas(string texto)
        {
            // Expresión regular para verificar que no haya más de 3 veces seguidas la misma letra
            string patron = @"(.)\1{2,}";

            // Verificar si el texto cumple con la expresión regular, considerando mayúsculas y minúsculas
            return !Regex.IsMatch(texto, patron, RegexOptions.IgnoreCase);
        }

        public static bool ApellidoValido(string apellido)
        {
            // Expresión regular para verificar que el apellido contenga solo dos palabras separadas por un espacio
            Regex regex = new Regex(@"^\p{L}+(?: \p{L}+)?$");

            // Verifica si el apellido coincide con la expresión regular
            return regex.IsMatch(apellido);
        }

        public static bool NoContieneEspaciosEnBlancoEnNumero(string numero)
        {
            // Verifica si el número contiene espacios en blanco
            return !numero.Contains(" ");
        }

        // Función para verificar que el correo electrónico no contenga espacios en blanco
        public static bool CorreoSinEspacios(string correo)
        {
            return !correo.Contains(' ');
        }

        // Función para verificar la estructura básica de un correo electrónico
        public static bool CorreoValidoEstructura(string correo)
        {
            // Expresión regular para verificar la estructura básica de un correo electrónico
            Regex regex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            return regex.IsMatch(correo);
        }

        public static bool CorreoValidoDominio(string correo)
        {
            // Lista de dominios válidos de proveedores populares
            var dominiosValidos = new List<string>
            {
                "gmail.com",
                "yahoo.com",
                "hotmail.com"
            };

            // Verifica si el correo contiene un símbolo de arroba (@)
            int atIndex = correo.IndexOf('@');
            if (atIndex == -1)
            {
                // Si no hay símbolo de arroba, el correo es inválido
                return false;
            }

            // Extrae el dominio del correo (todo lo que viene después de '@')
            string dominio = correo.Substring(atIndex + 1).ToLower(); // Convertimos a minúsculas para comparar de forma insensible a mayúsculas

            // Verifica si el dominio es uno de los dominios válidos
            return dominiosValidos.Contains(dominio);
        }

        public static bool CorreoArrobas(string correo)
        {
            // Contar la cantidad de arrobas (@) en el correo
            int count = correo.Count(c => c == '@');
            // El correo es válido si contiene exactamente una arroba
            return count == 1;
        }

        public static bool CorreoTresLetras(string correo)
        {
            // Verifica si hay al menos un símbolo de arroba (@)
            int atIndex = correo.IndexOf('@');
            if (atIndex == -1)
            {
                return false;
            }

            // Verifica que haya al menos 3 caracteres antes del símbolo de arroba
            return atIndex >= 3;
        }

        public static bool TelefonoValido(string telefono)
        {
            // Expresión regular para verificar que el número de teléfono no contenga cuatro o más ceros repetidos seguidos
            Regex regex = new Regex(@"^(?!.*0{5,})[0-9]{8}$");

            // Verifica si el número de teléfono coincide con la expresión regular
            return regex.IsMatch(telefono);
        }

        public static bool NombreV(string nombre)
        {
            // Verificar si el nombre tiene al menos 3 caracteres
            return nombre.Length >= 3 && !string.IsNullOrWhiteSpace(nombre) && !char.IsWhiteSpace(nombre[0]) && !nombre.Contains("  ");
        }

        public static bool ApellidoV(string apellido)
        {
            // Verificar si el apellido tiene al menos 3 caracteres
            return apellido.Length >= 3 && !string.IsNullOrWhiteSpace(apellido) && !char.IsWhiteSpace(apellido[0]) && !apellido.Contains("  ");
        }

        public static bool ValidarImpresora(string selectedPrinter)
        {
            if (string.IsNullOrEmpty(selectedPrinter))
            {
                MessageBox.Show("Por favor, seleccione una impresora.");
                return false;
            }
            return true;
        }

        public static bool ValidarPrintQueue(PrintQueue printQueue)
        {
            if (printQueue == null)
            {
                MessageBox.Show("Error al obtener la cola de impresión.");
                return false;
            }

            if (printQueue.IsOffline)
            {
                MessageBox.Show("La impresora está desconectada o fuera de línea.");
                return false;
            }
            return true;
        }

        public static bool ValidarCopias(string copiesText, out int copies)
        {
            if (string.IsNullOrEmpty(copiesText) || !int.TryParse(copiesText, out copies) || copies <= 0)
            {
                MessageBox.Show("Por favor, ingrese un número válido de copias.");
                copies = 0;
                return false;
            }
            return true;
        }

        public static string FormatearTexto(string texto)
        {
            // Verifica si el texto no es nulo ni vacío
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            // Obtiene la cultura actual del hilo en ejecución
            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            // Obtiene un objeto TextInfo que contiene información sobre el formato de texto
            TextInfo textInfo = cultureInfo.TextInfo;

            // Convierte el texto a minúsculas y luego a formato de título
            return textInfo.ToTitleCase(texto.ToLower());
        }

        public static void BloquearControles(KeyEventArgs e)
        {
            // Comprobar si se está presionando la tecla Ctrl
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Si es una combinación de teclas Ctrl+C, Ctrl+V o Ctrl+X, bloquearla
                if (e.Key == Key.C || e.Key == Key.V || e.Key == Key.X)
                {
                    e.Handled = true; // Detiene el evento
                }
            }
        }

        public static bool ValidarLongitudIDEmpleado(string idEmpleado)
        {
            // Verificar si el ID del empleado tiene más de 7 caracteres
            if (idEmpleado.Length > 7)
            {
                return true; // ID es demasiado largo
            }
            else
            {
                return false; // ID es de longitud válida
            }
        }

        public static bool EsIDEmpleadoValido(string idEmpleado)
        {
            // Verifica que cada caracter en el ID del empleado sea un dígito numérico
            foreach (char c in idEmpleado)
            {
                if (!char.IsDigit(c))
                {
                    // Si el caracter no es un dígito, el ID no es válido
                    return false;
                }
            }
            // Si todos los caracteres son dígitos, el ID es válido
            return true;
        }
    }
}
