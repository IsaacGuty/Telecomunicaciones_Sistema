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

        public CambContra()
        {
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Genera un código de verificación aleatorio
                int codigo = GenerarCodVerif();

                // Verifica si los campos de usuario y correo están vacíos
                if (Validaciones.CamposCorreoUsuarioVacios(txtUsuario.Text, txtCorreoE.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string usuario = txtUsuario.Text;
                string correo = txtCorreoE.Text;

                // Verifica que el correo tenga al menos tres letras antes del símbolo de arroba
                if (!Validaciones.CorreoTresLetras(correo))
                {
                    MessageBox.Show("El correo electrónico debe tener al menos tres letras antes del símbolo de arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica que el correo no contenga más de una arroba
                if (!Validaciones.CorreoArrobas(correo))
                {
                    MessageBox.Show("El correo electrónico no puede contener más de un arroba (@).", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Valida el dominio del correo electrónico
                if (!Validaciones.CorreoValidoDominio(correo))
                {
                    MessageBox.Show("El dominio del correo electrónico no es válido. Debe ser uno de los siguientes: gmail.com, yahoo.com, hotmail.com.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el usuario existe en la base de datos
                if (!InicioDAL.UsuarioExiste(usuario))
                {
                    MessageBox.Show("El usuario proporcionado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el correo electrónico está registrado en la base de datos
                if (!EmpleadoDAL.CorreoRegistrado(correo))
                {
                    MessageBox.Show("El correo electrónico proporcionado no está registrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!InicioDAL.CorreoUsuario(usuario, correo))
                {
                    MessageBox.Show("El correo electrónico no pertenece al usuario proporcionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Envía el código de verificación por correo electrónico
                EnviarCorreo(correo, codigo);

                // Registra la actividad de envío de código de verificación
                RegistrarActividad("Envío de código de verificación", usuario, correo);

                // Muestra un mensaje de éxito al usuario
                MessageBox.Show($"Se ha enviado un código de verificación al correo electrónico asociado al usuario.", "Código de verificación", MessageBoxButton.OK, MessageBoxImage.Information);

                // Oculta la ventana actual
                this.Hide();

                // Abre la ventana IngCod
                IngCod winCod = new IngCod(codigo.ToString(), correo, usuario, userId, false);
                winCod.ShowDialog();

                // Cierra la ventana actual
                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al conectarse a la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para generar un código de verificación aleatorio
        private int GenerarCodVerif()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        // Método para registrar la actividad en un archivo de registro
        private void RegistrarActividad(string actividad, string usuario, string correo)
        {
            string logMessage = $"{DateTime.Now}: {actividad} - Usuario: {usuario}, Correo: {correo}";
            string filePath = "log.txt";
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }

        // Método para enviar un correo electrónico con un código de verificación
        private void EnviarCorreo(string destinatario, int codigo)
        {
            using (SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587))
            {
                clienteSmtp.EnableSsl = true;
                clienteSmtp.UseDefaultCredentials = false;
                clienteSmtp.Credentials = new NetworkCredential(remitente, contraseña);

                using (MailMessage mensaje = new MailMessage(remitente, destinatario))
                {
                    mensaje.Subject = "Código de verificación";
                    mensaje.Body = $"Tu código de verificación es: {codigo}";

                    clienteSmtp.Send(mensaje);
                }
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            // Mostrar la nueva instancia de MainWindow
            mainWindow.Show();

            // Cerrar la ventana actual
            this.Close();

        }

        private void txtUsuario_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si el carácter ingresado es un número
            if (!char.IsDigit(e.Text, 0))
            {
                // Si el carácter no es un número, marca el evento como manejado para evitar que se agregue
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("Solo se permiten números en el campo de usuario.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Verifica si el texto resultante después de agregar el nuevo carácter excederá la longitud máxima permitida
            if (textBox.Text.Length + e.Text.Length > 7)
            {
                // Si excede la longitud máxima permitida, marca el evento como manejado para evitar que se agregue el nuevo carácter
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("El usuario no puede contener más de 7 caracteres.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtUsuario_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void txtCorreoE_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                // Marca el evento como manejado para evitar que se agregue el espacio
                e.Handled = true;

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

