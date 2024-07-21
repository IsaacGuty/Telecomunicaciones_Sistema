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
    public class Validaciones
    {
        public static bool CamposVacios(string usuario, string contraseña)
        {
            // Verifica si ambos campos, 'usuario' y 'contraseña', están vacíos o contienen solo espacios en blanco.
            return string.IsNullOrWhiteSpace(usuario) && string.IsNullOrWhiteSpace(contraseña);
        }

        public static bool UsuarioVacio(string usuario)
        {
            // Verifica si el campo 'usuario' está vacío o contiene solo espacios en blanco.
            return string.IsNullOrWhiteSpace(usuario);
        }

        public static bool ContraseñaVacia(string contraseña)
        {
            // Verifica si el campo 'contraseña' está vacío o contiene solo espacios en blanco.
            return string.IsNullOrWhiteSpace(contraseña);
        }

        public static bool ContieneSoloNumeros(string texto)
        {
            // Itera sobre cada carácter en la cadena 'texto'.
            foreach (char c in texto)
            {
                // Si el carácter no es un dígito, devuelve false.
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            // Si todos los caracteres son dígitos, devuelve true.
            return true;
        }

        public static bool CamposContraseñaCompletos(string anteriorContra, string nuevaContra, string confirmarContra)
        {
            // Verifica si alguno de los campos de contraseña está vacío.
            return !(string.IsNullOrEmpty(anteriorContra) || string.IsNullOrEmpty(nuevaContra) || string.IsNullOrEmpty(confirmarContra));
        }

        public static bool ContraseñasCoinciden(string nuevaContra, string confirmarContra)
        {
            // Compara 'nuevaContra' y 'confirmarContra' para verificar si son iguales.
            return nuevaContra == confirmarContra;
        }

        public static bool CamposCorreoUsuarioVacios(string usuario, string correo)
        {
            // Verifica si el valor de 'usuario' es nulo o una cadena vacía
            return string.IsNullOrEmpty(usuario)
                // Verifica si el valor de 'correo' es nulo o una cadena vacía
                || string.IsNullOrEmpty(correo);
        }

        public static bool IsGerenteGeneral(string rol)
        {
            // Compara 'rol' con la cadena "Gerente General" para verificar si el rol es Gerente General
            return rol == "Gerente General";
        }

        public static bool IsGerenteTecnico(string rol)
        {
            // Compara 'rol' con la cadena "Gerente Tecnico" para verificar si el rol es Gerente Tecnico
            return rol == "Gerente Técnico";
        }

        public static bool IsTecnico(string rol)
        {
            // Compara 'rol' con la cadena "Tecnico" para verificar si el rol es Tecnico
            return rol == "Técnico";
        }

        public static bool IsSecretaria(string rol)
        {
            // Compara 'rol' con las cadenas "Secretaria" o "Secretario" para verificar si el rol es Secretaria o Secretario
            return rol == "Secretaria" || rol == "Secretario";
        }

        public static bool IsContadora(string rol)
        {
            // Compara 'rol' con las cadenas "Contadora" o "Contador" para verificar si el rol es Contadora o Contador
            return rol == "Contadora" || rol == "Contador";
        }

        public static bool CamposClienteVacios(string nombre, string apellido, string telefono, string correo, string direccion, ComboBox cmbDire)
        {
            // Verifica si el campo 'nombre' es nulo o está vacío
            return string.IsNullOrWhiteSpace(nombre) ||
                   // Verifica si el campo 'apellido' es nulo o está vacío
                   string.IsNullOrWhiteSpace(apellido) ||
                   // Verifica si el campo 'telefono' es nulo o está vacío
                   string.IsNullOrWhiteSpace(telefono) ||
                   // Verifica si el campo 'correo' es nulo o está vacío
                   string.IsNullOrWhiteSpace(correo) ||
                   // Verifica si el campo 'direccion' es nulo o está vacío
                   string.IsNullOrWhiteSpace(direccion) ||
                   // Verifica si no se ha seleccionado ningún ítem en el ComboBox 'cmbDire'
                   cmbDire.SelectedItem == null;
        }

        public static bool EsTelefonoValido(string telefono)
        {
            // Verifica si el teléfono tiene 8 dígitos y comienza con 3, 8 o 9
            return telefono.Length == 8 && (telefono.StartsWith("3") || telefono.StartsWith("8") || telefono.StartsWith("9"));
        }

        public static bool CamposEmpleadosVacios(string idEmpleado, string nombre, string apellido, string telefono, string correo, string direccion)
        {
            // Verifica si el campo 'idEmpleado' es nulo o está vacío
            return string.IsNullOrWhiteSpace(idEmpleado) ||
                   // Verifica si el campo 'nombre' es nulo o está vacío
                   string.IsNullOrWhiteSpace(nombre) ||
                   // Verifica si el campo 'apellido' es nulo o está vacío
                   string.IsNullOrWhiteSpace(apellido) ||
                   // Verifica si el campo 'telefono' es nulo o está vacío
                   string.IsNullOrWhiteSpace(telefono) ||
                   // Verifica si el campo 'correo' es nulo o está vacío
                   string.IsNullOrWhiteSpace(correo) ||
                   // Verifica si el campo 'direccion' es nulo o está vacío
                   string.IsNullOrWhiteSpace(direccion);
        }

        public static bool CamposPagoVacios(string idPago, string idCliente, string nombreEmpleado, ComboBox cmbMes, string monto)
        {
            // Verifica si el campo 'idPago' no está vacío
            return !string.IsNullOrEmpty(idPago) &&
                   // Verifica si el campo 'idCliente' no está vacío
                   !string.IsNullOrEmpty(idCliente) &&
                   // Verifica si el campo 'nombreEmpleado' no está vacío
                   !string.IsNullOrEmpty(nombreEmpleado) &&
                   // Verifica si se ha seleccionado un elemento en el ComboBox 'cmbMes'
                   cmbMes.SelectedItem != null &&
                   // Verifica si el campo 'monto' no está vacío
                   !string.IsNullOrEmpty(monto);
        }

        public static bool CamposOrdenVacios(string nombre, string apellido, string direccion, string telefono, string servicio, object tipoT, object nombreE, object transporte)
        {
            // Verifica si el campo 'nombre' está vacío
            if (string.IsNullOrEmpty(nombre) ||
                // Verifica si el campo 'apellido' está vacío
                string.IsNullOrEmpty(apellido) ||
                // Verifica si el campo 'direccion' está vacío
                string.IsNullOrEmpty(direccion) ||
                // Verifica si el campo 'telefono' está vacío
                string.IsNullOrEmpty(telefono) ||
                // Verifica si el campo 'servicio' está vacío
                string.IsNullOrEmpty(servicio) ||
                // Verifica si el campo 'tipoT' es nulo
                tipoT == null ||
                // Verifica si el campo 'nombreE' es nulo
                nombreE == null ||
                // Verifica si el campo 'transporte' es nulo
                transporte == null)
            {
                // Retorna falso si alguno de los campos está vacío o es nulo
                return false;
            }
            // Retorna verdadero si todos los campos no están vacíos o nulos
            return true;
        }

        public static bool NoContieneEspaciosEnBlanco(string texto)
        {
            // Verifica si el texto no está vacío, no es solo espacios en blanco y su longitud es mayor a 0 después de quitar espacios al principio y al final
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
            // Expresión regular para verificar que el número de teléfono no contenga seis o más dígitos repetidos seguidos
            Regex regex = new Regex(@"^(?!.*(\d)\1{5,})\d{8}$");

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
            // Verifica si la cadena 'selectedPrinter' es nula o está vacía.
            if (string.IsNullOrEmpty(selectedPrinter))
            {
                // Muestra un mensaje indicando que se debe seleccionar una impresora.
                MessageBox.Show("Por favor, seleccione una impresora.");
                // Retorna false ya que la impresora no está seleccionada.
                return false;
            }
            // Retorna true si la impresora está seleccionada.
            return true;
        }

        public static bool ValidarPrintQueue(PrintQueue printQueue)
        {
            // Verifica si 'printQueue' es nulo.
            if (printQueue == null)
            {
                // Muestra un mensaje indicando que hubo un error al obtener la cola de impresión.
                MessageBox.Show("Error al obtener la cola de impresión.");
                // Retorna false ya que la cola de impresión es nula.
                return false;
            }

            // Verifica si la impresora está desconectada o fuera de línea.
            if (printQueue.IsOffline)
            {
                // Muestra un mensaje indicando que la impresora está desconectada o fuera de línea.
                MessageBox.Show("La impresora está desconectada o fuera de línea.");
                // Retorna false ya que la impresora está desconectada o fuera de línea.
                return false;
            }
            // Retorna true si la cola de impresión no es nula y está en línea.
            return true;
        }

        public static bool ValidarCopias(string copiesText, out int copies)
        {
            // Intenta convertir el texto 'copiesText' a un número entero y verifica si es mayor que 0.
            if (string.IsNullOrEmpty(copiesText) || !int.TryParse(copiesText, out copies) || copies <= 0)
            {
                // Muestra un mensaje indicando que se debe ingresar un número válido de copias.
                MessageBox.Show("Por favor, ingrese un número válido de copias.");
                // Asigna 0 a 'copies' en caso de que la validación falle.
                copies = 0;
                // Retorna false ya que el número de copias no es válido.
                return false;
            }
            // Retorna true si el número de copias es un entero positivo válido.
            return true;
        }

        public static string FormatearTexto(string texto)
        {
            // Verifica si el texto no es nulo ni vacío
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture; // Obtiene la cultura actual del hilo en ejecución

            TextInfo textInfo = cultureInfo.TextInfo; // Obtiene un objeto TextInfo que contiene información sobre el formato de texto

            return textInfo.ToTitleCase(texto.ToLower()); // Convierte el texto a minúsculas y luego a formato de título
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

        public static bool BusquedaEValida(string texto, out string mensaje)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensaje = string.Empty;

            // Verifica si el texto es nulo, vacío o es igual a "ID, nombre, apellido".
            if (string.IsNullOrEmpty(texto) || texto == "ID, nombre, apellido")
            {
                // Si es así, asigna un mensaje de error indicando que se debe ingresar un valor válido.
                mensaje = "Debe ingresar el ID_Empleado, Nombre o Apellido, para realizar la búsqueda.";
                // Devuelve false para indicar que la búsqueda no es válida.
                return false;
            }

            // Si el texto es válido, devuelve true.
            return true;
        }

        public static bool BusquedaCValida(string texto, out string mensaje)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensaje = string.Empty;

            // Verifica si el texto es nulo, vacío o es igual a "ID, nombre, apellido".
            if (string.IsNullOrEmpty(texto) || texto == "ID, nombre, apellido")
            {
                // Si es así, asigna un mensaje de error indicando que se debe ingresar un valor válido.
                mensaje = "Debe ingresar el ID_Cliente, Nombre o Apellido, para realizar la búsqueda.";
                // Devuelve false para indicar que la búsqueda no es válida.
                return false;
            }

            // Si el texto es válido, devuelve true.
            return true;
        }

        public static bool BusquedaTValida(string texto, out string mensaje)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensaje = string.Empty;

            // Verifica si el texto es nulo, vacío o es igual a "ID Placa, Marca".
            if (string.IsNullOrEmpty(texto) || texto == "ID Placa, Marca")
            {
                // Si es así, asigna un mensaje de error indicando que se debe ingresar un valor válido.
                mensaje = "Debe ingresar el ID_Placa o Marca, para realizar la búsqueda.";
                // Devuelve false para indicar que la búsqueda no es válida.
                return false;
            }

            // Si el texto es válido, devuelve true.
            return true;
        }

        public static bool BusquedaOValida(string texto, out string mensaje)
        {
            // Inicializa el mensaje a una cadena vacía
            mensaje = string.Empty;

            // Verifica si el texto es nulo o vacío, o si el texto es "Nombre, apellido"
            if (string.IsNullOrEmpty(texto) || texto == "Nombre, apellido")
            {
                // Establece un mensaje de error si la condición anterior se cumple
                mensaje = "Debe ingresar el Nombre o Apellido, para realizar la búsqueda.";

                // Retorna false indicando que el texto no es válido para la búsqueda
                return false;
            }

            // Retorna true indicando que el texto es válido para la búsqueda
            return true;
        }

        public static bool ValidarTelefonoLongCar(string textoActual, string nuevoTexto, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Verifica si el texto resultante después de agregar el nuevo carácter excederá la longitud máxima permitida
            if (textoActual.Length + nuevoTexto.Length > 8)
            {
                mensajeError = "El número de teléfono no puede tener más de 8 dígitos.";
                return false;
            }

            // Verifica si el carácter ingresado es un dígito
            if (!char.IsDigit(nuevoTexto, 0))
            {
                if (char.IsLetter(nuevoTexto, 0))
                {
                    mensajeError = "No se permiten letras en el número de teléfono.";
                }
                else
                {
                    mensajeError = "No se permiten caracteres especiales en el número de teléfono.";
                }
                return false;
            }

            return true;
        }

        public static bool ValidarColor(string color, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Verificar longitud máxima de 20 caracteres
            if (color.Length > 20)
            {
                mensajeError = "Se ha alcanzado el límite máximo de 20 caracteres en el campo de color.";
                return false;
            }

            // Verificar que solo contenga letras y espacios
            if (!Regex.IsMatch(color, @"^[a-zA-Z ]+$"))
            {
                mensajeError = "El color no puede contener caracteres especiales ni números.";
                return false;
            }

            // Verificar que no contenga números
            if (Regex.IsMatch(color, @"\d"))
            {
                mensajeError = "El color no puede contener números.";
                return false;
            }

            return true;
        }

        public static bool ValidarTeclaEspacioTel(KeyEventArgs e, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                mensajeError = "No se permiten espacios en blanco en el número de teléfono.";
                return false;
            }

            return true;
        }

        public static bool ValidarTeclaEspacioCorr(KeyEventArgs e, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (e.Key == Key.Space)
            {
                // Verifica si la tecla presionada es la barra espaciadora
                mensajeError = "No se permiten espacios en blanco en el campo de correo.";
                return false;
            }

            return true;
        }

        public static bool ValidarTeclaEspacioIDE(KeyEventArgs e, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (e.Key == Key.Space)
            {
                // Verifica si la tecla presionada es la barra espaciadora
                mensajeError = "No se permiten espacios en blanco en el ID del empleado.";
                return false;
            }

            return true;
        }

        public static bool ValidarTeclaEspacioIDP(KeyEventArgs e, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (e.Key == Key.Space)
            {
                // Verifica si la tecla presionada es la barra espaciadora
                mensajeError = "No se permiten espacios en blanco en el Año.";
                return false;
            }

            return true;
        }

        public static bool ValidarNombreLongNumCar(string textoActual, string nuevoTexto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud total del texto actual más el nuevo texto supera los 50 caracteres.
            if (textoActual.Length + nuevoTexto.Length > 50)
            {
                mensajeError = "Se ha alcanzado el límite máximo de 50 caracteres en el campo de nombre.";
                return false;
            }

            // Utiliza una expresión regular para comprobar si el nuevo texto contiene números.
            if (System.Text.RegularExpressions.Regex.IsMatch(nuevoTexto, "[0-9]"))
            {
                mensajeError = "No se permiten números en el campo de nombre.";
                return false;
            }

            // Utiliza una expresión regular para comprobar si el nuevo texto contiene caracteres especiales.
            if (System.Text.RegularExpressions.Regex.IsMatch(nuevoTexto, "[^a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
            {
                mensajeError = "No se permiten caracteres especiales en el campo de nombre.";
                return false;
            }

            // Si todas las verificaciones pasan, retorna verdadero indicando que la validación fue exitosa.
            return true;
        }

        public static bool ValidarApellidoLongNumCar(string textoActual, string nuevoTexto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud total del texto actual más el nuevo texto supera los 50 caracteres.
            if (textoActual.Length + nuevoTexto.Length > 50)
            {
                mensajeError = "Se ha alcanzado el límite máximo de 50 caracteres en el campo de apellido.";
                return false;
            }

            // Utiliza una expresión regular para comprobar si el nuevo texto contiene números.
            if (System.Text.RegularExpressions.Regex.IsMatch(nuevoTexto, "[0-9]"))
            {
                mensajeError = "No se permiten números en el campo de apellido.";
                return false;
            }

            // Utiliza una expresión regular para comprobar si el nuevo texto contiene caracteres especiales.
            if (System.Text.RegularExpressions.Regex.IsMatch(nuevoTexto, "[^a-zA-ZáéíóúÁÉÍÓÚñÑ]"))
            {
                mensajeError = "No se permiten caracteres especiales en el campo de apellido.";
                return false;
            }

            // Si todas las verificaciones pasan, retorna verdadero indicando que la validación fue exitosa.
            return true;
        }

        public static bool ValidarCorreoLongitud(string textoActual, string nuevoTexto, out string mensajeError)
        {
            // Inicializa el parámetro de salida mensajeError como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud combinada del textoActual y el nuevoTexto excede 50 caracteres.
            if (textoActual.Length + nuevoTexto.Length > 50)
            {
                // Si la longitud combinada supera 50, asigna un mensaje de error y retorna falso.
                mensajeError = "Se ha alcanzado el límite máximo de 50 caracteres en el campo de correo.";
                return false;
            }

            // Si la longitud combinada está dentro del límite, retorna verdadero.
            return true;
        }

        public static bool ValidarContraseñaEspLong(string contraseñaActual, KeyEventArgs e, out string mensajeError)
        {
            // Inicializa el parámetro de salida mensajeError como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la tecla presionada es un espacio en blanco.
            if (e.Key == Key.Space)
            {
                // Si la tecla es un espacio en blanco, asigna un mensaje de error y retorna falso.
                mensajeError = "No se permiten espacios en blanco en la contraseña.";
                return false;
            }

            // Verifica si la longitud de la contraseña actual es mayor o igual a 12 y la tecla presionada no es un carácter de control.
            if (contraseñaActual.Length >= 12 && !char.IsControl((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                // Si la longitud es adecuada y la tecla no es un carácter de control, asigna un mensaje de error y retorna falso.
                mensajeError = "La contraseña no puede tener más de 12 caracteres.";
                return false;
            }

            // Si las condiciones anteriores no se cumplen, retorna verdadero.
            return true;
        }

        // Método que verifica si un texto contiene espacios en blanco
        public static bool ContieneEspaciosContraseñaA(string texto)
        {
            // Retorna verdadero si el texto contiene al menos un espacio en blanco; de lo contrario, retorna falso.
            return texto.Contains(" ");
        }

        // Método que verifica si la longitud de un texto excede la longitud máxima permitida
        public static bool ExcedeLongitudMaximaContraseñaA(string texto, int longitudMaxima)
        {
            // Retorna verdadero si la longitud del texto es mayor o igual a la longitud máxima; de lo contrario, retorna falso.
            return texto.Length >= longitudMaxima;
        }

        // Método que valida una placa de vehículo y proporciona un mensaje de error si no es válida
        public static bool ValidarPlaca(string placa, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía
            mensajeError = string.Empty;

            // Verifica si la longitud de la placa es exactamente 7 caracteres
            if (placa.Length != 7)
            {
                // Si la longitud es incorrecta, establece el mensaje de error y retorna falso
                mensajeError = "La placa debe tener exactamente 7 caracteres.";
                return false;
            }

            // Extrae los primeros 3 caracteres de la placa para verificar las letras
            string letras = placa.Substring(0, 3);
            // Extrae los últimos 4 caracteres de la placa para verificar los números
            string numeros = placa.Substring(3, 4);

            // Verifica si las primeras 3 letras son exactamente 3 letras mayúsculas
            if (!Regex.IsMatch(letras, @"^[A-Z]{3}$"))
            {
                // Si la verificación falla, establece el mensaje de error y retorna falso
                mensajeError = "La placa debe comenzar con exactamente 3 letras mayúsculas.";
                return false;
            }

            // Verifica si los últimos 4 caracteres son exactamente 4 dígitos
            if (!Regex.IsMatch(numeros, @"^\d{4}$"))
            {
                // Si la verificación falla, establece el mensaje de error y retorna falso
                mensajeError = "La placa debe terminar con exactamente 4 números.";
                return false;
            }

            // Si todas las validaciones son correctas, retorna verdadero
            return true;
        }

        public static bool MMValido(string nombre)
        {
            // Expresión regular para verificar que el nombre contenga solo dos palabras separadas por un espacio
            Regex regex = new Regex(@"^\p{L}+(?: \p{L}+)?$");

            // Verifica si el nombre coincide con la expresión regular
            return regex.IsMatch(nombre);
        }

        // Función para validar un IDPC.
        public static bool ValidarIDPC(string texto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud del texto es mayor a 7 caracteres.
            if (texto.Length > 7)
            {
                // Si la longitud excede el límite, asigna un mensaje de error adecuado.
                mensajeError = "No se permiten más de 7 caracteres.";
                // Retorna falso indicando que la validación falló.
                return false;
            }

            // Verifica si el texto contiene solo caracteres alfanuméricos.
            if (!Regex.IsMatch(texto, @"^[a-zA-Z0-9]+$"))
            {
                // Si el texto contiene caracteres especiales, asigna un mensaje de error adecuado.
                mensajeError = "El IDP no puede contener caracteres especiales.";
                // Retorna falso indicando que la validación falló.
                return false;
            }

            // Retorna verdadero indicando que la validación fue exitosa.
            return true;
        }

        // Función para validar una marca.
        public static bool ValidarMarca(string texto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud del texto es mayor a 20 caracteres.
            if (texto.Length > 20)
            {
                // Si la longitud excede el límite, asigna un mensaje de error adecuado.
                mensajeError = "Se ha alcanzado el límite máximo de 20 caracteres en el campo de marca.";
                // Retorna falso indicando que la validación falló.
                return false;
            }

            // Retorna verdadero indicando que la validación fue exitosa.
            return true;
        }

        // Función para validar un modelo.
        public static bool ValidarModelo(string texto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si la longitud del texto es mayor a 20 caracteres.
            if (texto.Length > 20)
            {
                // Si la longitud excede el límite, asigna un mensaje de error adecuado.
                mensajeError = "Se ha alcanzado el límite máximo de 20 caracteres en el campo de modelo.";
                // Retorna falso indicando que la validación falló.
                return false;
            }

            // Retorna verdadero indicando que la validación fue exitosa.
            return true;
        }

        public static bool ValidarAño(string texto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía
            mensajeError = string.Empty;

            // Verifica si la longitud del texto es mayor a 4 caracteres
            if (texto.Length > 4)
            {
                // Asigna un mensaje de error si hay más de 4 caracteres
                mensajeError = "No se permiten más de 4 caracteres.";
                // Retorna false indicando que la validación ha fallado
                return false;
            }

            // Verifica si el texto contiene letras
            if (Regex.IsMatch(texto, @"[a-zA-Z]"))
            {
                // Asigna un mensaje de error si el texto contiene letras
                mensajeError = "El año no puede contener letras.";
                // Retorna false indicando que la validación ha fallado
                return false;
            }

            // Verifica si el texto contiene caracteres especiales (todo lo que no sea un dígito)
            if (!Regex.IsMatch(texto, @"^[0-9]+$"))
            {
                // Asigna un mensaje de error si el texto contiene caracteres especiales
                mensajeError = "El año no puede contener caracteres especiales.";
                // Retorna false indicando que la validación ha fallado
                return false;
            }

            // Retorna true si todas las validaciones son exitosas
            return true;
        }

        public static bool ValidarUnDigito(string texto, string nuevoCaracter, out string mensajeError)
        {
            // Verifica si la longitud del texto concatenado con el nuevo carácter es mayor a 1
            if ((texto + nuevoCaracter).Length > 1)
            {
                // Asigna un mensaje de error si la longitud es mayor a 1
                mensajeError = "Solo se permite un dígito.";
                // Retorna false indicando que la validación ha fallado
                return false;
            }
            // Inicializa el mensaje de error como una cadena vacía si la validación es exitosa
            mensajeError = string.Empty;
            // Retorna true indicando que la validación ha sido exitosa
            return true;
        }

        public static bool ValidarNoLetra(string texto, out string mensajeError)
        {
            // Verifica si el último carácter del texto es una letra
            if (char.IsLetter(texto, texto.Length - 1))
            {
                // Asigna un mensaje de error si el último carácter es una letra
                mensajeError = "No se aceptan letras.";
                // Retorna false indicando que la validación ha fallado
                return false;
            }
            // Inicializa el mensaje de error como una cadena vacía si la validación es exitosa
            mensajeError = string.Empty;
            // Retorna true indicando que la validación ha sido exitosa
            return true;
        }

        public static bool ValidarNoCaracterEspecial(string texto, out string mensajeError)
        {
            // Verifica si el último carácter del texto no es una letra ni un dígito.
            if (!char.IsLetterOrDigit(texto, texto.Length - 1))
            {
                // Establece el mensaje de error si se encuentra un carácter especial.
                mensajeError = "No se aceptan caracteres especiales.";
                // Retorna false indicando que la validación falló.
                return false;
            }
            // Establece un mensaje vacío si no se encontraron errores.
            mensajeError = string.Empty;
            // Retorna true indicando que la validación pasó.
            return true;
        }

        public static bool ValidarNoEspacio(Key key, out string mensajeError)
        {
            // Verifica si la tecla presionada es la tecla de espacio.
            if (key == Key.Space)
            {
                // Establece el mensaje de error si se presiona la tecla de espacio.
                mensajeError = "Los espacios en blanco no están permitidos.";
                // Retorna false indicando que la validación falló.
                return false;
            }
            // Establece un mensaje vacío si no se presionó la tecla de espacio.
            mensajeError = string.Empty;
            // Retorna true indicando que la validación pasó.
            return true;
        }

        public static bool ValidarNoEspacioUsuario(KeyEventArgs e, out string mensajeError)
        {
            // Establece un mensaje vacío inicialmente.
            mensajeError = string.Empty;

            // Verifica si la tecla presionada es la tecla de espacio.
            if (e.Key == Key.Space)
            {
                // Establece el mensaje de error si se presiona la tecla de espacio.
                mensajeError = "No se permiten espacios en blanco en el campo de usuario.";
                // Retorna false indicando que la validación falló.
                return false;
            }

            // Retorna true indicando que la validación pasó.
            return true;
        }

        public static bool ValidarNoEspacioCorreo(KeyEventArgs e, out string mensajeError)
        {
            // Establece un mensaje vacío inicialmente.
            mensajeError = string.Empty;

            // Verifica si la tecla presionada es la tecla de espacio.
            if (e.Key == Key.Space)
            {
                // Establece el mensaje de error si se presiona la tecla de espacio.
                mensajeError = "No se permiten espacios en blanco en el campo de correo.";
                // Retorna false indicando que la validación falló.
                return false;
            }

            // Retorna true indicando que la validación pasó.
            return true;
        }

        public static bool ValidarTextoUsuario(string textoActual, string nuevoCaracter, out string mensajeError)
        {
            mensajeError = string.Empty; // Inicializa el mensaje de error vacío.

            // Verifica si el nuevo carácter es un dígito. Si no lo es:
            if (!char.IsDigit(nuevoCaracter, 0))
            {
                // Verifica si el nuevo carácter es una letra. Si es así:
                if (char.IsLetter(nuevoCaracter, 0))
                {
                    mensajeError = "No se permiten letras en el campo de usuario."; // Establece el mensaje de error para letras.
                }
                else
                {
                    mensajeError = "No se permiten caracteres especiales en el campo de usuario."; // Establece el mensaje de error para caracteres especiales.
                }
                return false; // Retorna false ya que el carácter no es válido.
            }

            // Verifica si la longitud total del texto actual más el nuevo carácter excede la longitud máxima permitida de 7 caracteres.
            if (textoActual.Length + nuevoCaracter.Length > 7)
            {
                mensajeError = "El usuario no puede contener más de 7 caracteres."; // Establece el mensaje de error para longitud excesiva.
                return false; // Retorna false ya que la longitud excede el límite.
            }

            return true; // Retorna true ya que el texto es válido.
        }

        public static bool ValidarLongitudTextoID(TextBox textBox, string nuevoTexto, int longitudMaxima, out string mensajeError)
        {
            // Verifica si la longitud total del texto actual en el TextBox más el nuevo texto excede la longitud máxima permitida.
            if (textBox.Text.Length + nuevoTexto.Length > longitudMaxima)
            {
                mensajeError = "El ID del empleado no puede tener más de " + longitudMaxima + " dígitos."; // Establece el mensaje de error para longitud excesiva.
                return false; // Retorna false ya que la longitud excede el límite.
            }

            mensajeError = null; // Establece el mensaje de error como null ya que la validación es exitosa.
            return true; // Retorna true ya que la longitud es válida.
        }

        public static bool ValidarCaracteresID(string texto, out string mensajeError)
        {
            // Verifica si el primer carácter del texto no es un dígito.
            if (!char.IsDigit(texto, 0))
            {
                // Verifica si el primer carácter del texto es una letra. Si es así:
                if (char.IsLetter(texto, 0))
                {
                    mensajeError = "No se permiten letras en el ID del empleado."; // Establece el mensaje de error para letras.
                    return false; // Retorna false ya que el texto contiene letras.
                }
                else
                {
                    mensajeError = "No se permiten caracteres especiales en el ID del empleado."; // Establece el mensaje de error para caracteres especiales.
                    return false; // Retorna false ya que el texto contiene caracteres especiales.
                }
            }

            mensajeError = null; // Establece el mensaje de error como null ya que la validación es exitosa.
            return true; // Retorna true ya que el texto contiene solo dígitos.
        }

        public static bool TeclaBloqueada(Key tecla)
        {
            // En este caso, solo bloqueamos la barra espaciadora
            return tecla == Key.Space;
        }

        public static bool ValidarTextoConfirmacion(string texto, out string mensajeError)
        {
            // Inicializa el mensaje de error como una cadena vacía.
            mensajeError = string.Empty;

            // Verifica si el texto contiene espacios en blanco.
            // Si el texto contiene al menos un espacio, asigna un mensaje de error y retorna false.
            if (texto.Contains(" "))
            {
                mensajeError = "No se permiten espacios en blanco en la confirmación de contraseña.";
                return false;
            }

            // Verifica si la longitud del texto es mayor o igual a 12 caracteres.
            // Si es así, asigna un mensaje de error y retorna false.
            else if (texto.Length >= 12)
            {
                mensajeError = "Se permiten un máximo de 12 dígitos.";
                return false;
            }

            // Si el texto no contiene espacios y tiene una longitud menor a 12, la validación es exitosa.
            return true;
        }

        public static bool ValidarNuevaContraseña(string texto, string contrasenaActual, out string mensajeError)
        {
            // Inicializa el mensaje de error vacío
            mensajeError = string.Empty;

            // Verifica si el texto de la nueva contraseña contiene espacios en blanco
            if (texto.Contains(" "))
            {
                // Si contiene espacios en blanco, asigna un mensaje de error y devuelve false
                mensajeError = "No se permiten espacios en blanco en la nueva contraseña.";
                return false;
            }
            // Verifica si la longitud de la contraseña actual es mayor o igual a 12
            else if (contrasenaActual.Length >= 12)
            {
                // Si la longitud es mayor o igual a 12, asigna un mensaje de error y devuelve false
                mensajeError = "Se permiten un máximo de 12 dígitos.";
                return false;
            }

            // Si ninguna de las condiciones anteriores se cumple, la contraseña es válida y devuelve true
            return true;
        }

        public static void ValidarEspacios(KeyEventArgs e)
        {
            // Verifica si la tecla presionada es la barra espaciadora (Key.Space)
            if (e.Key == Key.Space)
            {
                // Establece la propiedad Handled del evento a true para indicar que el evento ha sido manejado
                e.Handled = true;

                // Muestra un cuadro de mensaje con un mensaje de error, un título y un botón de OK
                MessageBox.Show("No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
