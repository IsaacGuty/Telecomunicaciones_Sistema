using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para CambContra.xaml
    /// </summary>
    public partial class CambContra : Window
    {
        private readonly string remitente = "telecomunicacioness.2024@gmail.com"; // Dirección de correo electrónico del remitente
        private readonly string contraseña = "fast hqaz dejf uxro"; // Contraseña del remitente
        private int userId; // ID del usuario

        // Constructor de la ventana CambContra que acepta el userId como parámetro
        public CambContra(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Inicializa userId con el valor proporcionado
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Genera un código de verificación aleatorio
                int codigo = GenerarCodVerif(); // Llama a la función que genera un código de verificación aleatorio para el proceso de recuperación de cuenta.

                // Verifica si los campos de usuario y correo están vacíos
                if (Validaciones.CamposCorreoUsuarioVacios(txtUsuario.Text, txtCorreoE.Text))
                {
                    // Muestra un mensaje de error si uno o ambos campos están vacíos
                    MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si los campos están vacíos
                }

                string usuario = txtUsuario.Text; // Obtiene el texto ingresado en el campo de usuario
                string correo = txtCorreoE.Text;  // Obtiene el texto ingresado en el campo de correo electrónico

                // Verifica que el correo tenga al menos tres letras antes del símbolo de arroba
                if (!Validaciones.CorreoTresLetras(correo))
                {
                    // Muestra un mensaje de error si el correo no cumple con el requisito de tener al menos tres letras antes del '@'
                    MessageBox.Show("El correo electrónico debe tener al menos tres letras antes del símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el correo no cumple con el formato
                }

                // Verifica que el correo no contenga más de una arroba
                if (!Validaciones.CorreoArrobas(correo))
                {
                    // Muestra un mensaje de error si el correo contiene más de una '@'
                    MessageBox.Show("El correo electrónico no puede contener más de un arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el correo tiene más de una '@'
                }

                // Valida el dominio del correo electrónico
                if (!Validaciones.CorreoValidoDominio(correo))
                {
                    // Muestra un mensaje de error si el dominio del correo electrónico no es uno de los permitidos
                    MessageBox.Show("El dominio del correo electrónico no es válido. Debe ser uno de los siguientes: gmail.com, yahoo.com, hotmail.com.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el dominio del correo no es válido
                }

                // Verifica si el usuario existe en la base de datos
                if (!InicioDAL.UsuarioExiste(usuario))
                {
                    // Muestra un mensaje de error si el usuario proporcionado no existe en la base de datos
                    MessageBox.Show("El usuario proporcionado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el usuario no está en la base de datos
                }

                // Verifica si el correo electrónico está registrado en la base de datos
                if (!EmpleadoDAL.CorreoRegistrado(correo))
                {
                    // Muestra un mensaje de error si el correo electrónico proporcionado no está registrado en la base de datos
                    MessageBox.Show("El correo electrónico proporcionado no está registrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el correo no está registrado
                }

                // Verifica si el correo electrónico pertenece al usuario proporcionado
                if (!InicioDAL.CorreoUsuario(usuario, correo))
                {
                    // Muestra un mensaje de error si el correo electrónico no está asociado con el usuario proporcionado
                    MessageBox.Show("El correo electrónico no pertenece al usuario proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Termina la ejecución del método si el correo no coincide con el usuario
                }

                // Envía el código de verificación por correo electrónico
                EnviarCorreo(correo, codigo); // Llama a la función para enviar el código de verificación al correo electrónico del usuario

                // Registra la actividad de envío de código de verificación
                RegistrarActividad("Envío de código de verificación", usuario, correo); // Registra en el sistema que se ha enviado un código de verificación

                // Muestra un mensaje de éxito al usuario
                MessageBox.Show($"Se ha enviado un código de verificación al correo electrónico asociado al usuario.", "Código de verificación", MessageBoxButton.OK, MessageBoxImage.Information);

                // Oculta la ventana actual
                this.Hide(); // Oculta la ventana actual para que el usuario pueda proceder con el ingreso del código

                // Abre la ventana IngCod
                IngCod winCod = new IngCod(codigo.ToString(), correo, usuario, userId, false); // Crea una nueva ventana para ingresar el código de verificación
                winCod.ShowDialog(); // Muestra la ventana modal para que el usuario ingrese el código

                // Cierra la ventana actual
                this.Close(); // Cierra la ventana actual después de que el usuario haya ingresado el código
            }
            catch (SqlException ex)
            {
                // Maneja excepciones relacionadas con SQL (por ejemplo, problemas de conexión a la base de datos)
                MessageBox.Show("Error al conectarse a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SmtpException ex)
            {
                // Maneja excepciones relacionadas con el envío de correos electrónicos
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                // Maneja cualquier otra excepción que pueda ocurrir
                MessageBox.Show("Se produjo un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para generar un código de verificación aleatorio
        private int GenerarCodVerif()
        {
            // Crea una instancia de la clase Random para generar números aleatorios
            Random rnd = new Random();

            // Genera y retorna un número entero aleatorio entre 100000 y 999999, ambos inclusive.
            // El rango asegura que el código de verificación sea siempre un número de 6 dígitos.
            return rnd.Next(100000, 999999);
        }

        // Método para registrar la actividad en un archivo de registro
        private void RegistrarActividad(string actividad, string usuario, string correo)
        {
            // Crea un mensaje de registro que incluye la fecha y hora actual,
            // la descripción de la actividad, el nombre de usuario y el correo.
            string logMessage = $"{DateTime.Now}: {actividad} - Usuario: {usuario}, Correo: {correo}";

            // Define la ruta del archivo de registro donde se guardará el mensaje.
            string filePath = "log.txt";

            // Agrega el mensaje de registro al final del archivo especificado.
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }

        // Método para enviar un correo electrónico con un código de verificación
        private void EnviarCorreo(string destinatario, int codigo)
        {
            // Crear una instancia de SmtpClient para conectar con el servidor SMTP de Gmail
            using (SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587))
            {
                // Habilitar SSL para asegurar la conexión
                clienteSmtp.EnableSsl = true;

                // No usar las credenciales predeterminadas del sistema
                clienteSmtp.UseDefaultCredentials = false;

                // Establecer las credenciales para autenticar el remitente en el servidor SMTP
                clienteSmtp.Credentials = new NetworkCredential(remitente, contraseña);

                // Crear el mensaje de correo
                using (MailMessage mensaje = new MailMessage(remitente, destinatario))
                {
                    // Establecer el asunto del correo
                    mensaje.Subject = "Código de verificación";

                    // Establecer el cuerpo del correo con el código de verificación
                    mensaje.Body = $"Tu código de verificación es: {codigo}";

                    // Enviar el mensaje de correo
                    clienteSmtp.Send(mensaje);
                }
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Inicio_Sesión Inicio_Sesión = new Inicio_Sesión();

            // Mostrar la nueva instancia de Inicio_Sesión
            Inicio_Sesión.Show();

            // Cerrar la ventana actual
            this.Close();

        }

        private void txtUsuario_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string mensajeError;

            // Valida el texto ingresado en el campo de usuario
            if (!Validaciones.ValidarTextoUsuario(textBox.Text, e.Text, out mensajeError))
            {
                e.Handled = true; // Marca el evento como manejado para evitar la adición del texto

                // Muestra un mensaje de error al usuario
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtUsuario_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string mensajeError;

            // Valida la entrada de teclas en el campo de usuario
            if (!Validaciones.ValidarNoEspacioUsuario(e, out mensajeError))
            {
                e.Handled = true; // Marca el evento como manejado para evitar el espacio

                // Muestra un mensaje de error al usuario
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Bloquea controles si es necesario
            Validaciones.BloquearControles(e);
        }

        private void txtCorreoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            string mensajeError;

            // Valida la entrada de teclas en el campo de usuario
            if (!Validaciones.ValidarNoEspacioCorreo(e, out mensajeError))
            {
                e.Handled = true; // Marca el evento como manejado para evitar el espacio

                // Muestra un mensaje de error al usuario
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Bloquea controles si es necesario
            Validaciones.BloquearControles(e);
        }

        private void txtCorreoE_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Se realiza un casting del objeto sender a TextBox para poder trabajar con él.
            TextBox textBox = sender as TextBox;

            //Se verifica si el correo electrónico ingresado cumple con las reglas de longitud permitidas.
            if (!Validaciones.ValidarCorreoLongitud(textBox.Text, e.Text, out string mensajeError))
            {
                //Si la validación falla (es decir, si el correo electrónico no cumple con los criterios de longitud),
                // se marca el evento como manejado para evitar que el texto sea ingresado en el TextBox.
                e.Handled = true;

                //Se muestra un mensaje de advertencia al usuario indicando que el correo electrónico no cumple con las reglas de longitud.
                MessageBox.Show(mensajeError, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

