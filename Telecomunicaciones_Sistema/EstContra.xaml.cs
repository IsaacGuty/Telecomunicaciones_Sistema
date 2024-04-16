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
    public partial class EstContra : Window
    {
        // Dirección de correo electrónico del remitente
        private readonly string remitente = "telecomunicacioness.2024@gmail.com";

        // Contraseña del remitente
        private readonly string contraseña = "fast hqaz dejf uxro";

        // ID del usuario
        private int userId;

        // Constructor de la ventana EstContra
        public EstContra(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Inicializa userId con el valor proporcionado
        }

        public EstContra()
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

                // Valida el formato del correo electrónico
                if (!Validaciones.CorreoValido(correo))
                {
                    MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el usuario existe en la base de datos
                if (!Validaciones.UsuarioExiste(usuario))
                {
                    MessageBox.Show("El usuario proporcionado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Verifica si el correo electrónico está registrado en la base de datos
                if (!Validaciones.CorreoRegistrado(correo))
                {
                    MessageBox.Show("El correo electrónico proporcionado no está registrado en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                IngCod winCod = new IngCod(codigo.ToString(), correo, usuario, userId, true);
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

        // Genera un código de verificación aleatorio de 6 dígitos
        private int GenerarCodVerif()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        // Registra la actividad realizada en un archivo de registro
        private void RegistrarActividad(string actividad, string usuario, string correo)
        {
            // Formatea el mensaje de registro
            string logMessage = $"{DateTime.Now}: {actividad} - Usuario: {usuario}, Correo: {correo}";
            string filePath = "log.txt"; // Ruta del archivo de registro
            // Agrega el mensaje de registro al archivo
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }

        // Envía un correo electrónico con el código de verificación
        private void EnviarCorreo(string destinatario, int codigo)
        {
            using (SmtpClient clienteSmtp = new SmtpClient("smtp.gmail.com", 587))
            {
                clienteSmtp.EnableSsl = true; // Habilita SSL
                clienteSmtp.UseDefaultCredentials = false; // No se utilizan las credenciales predeterminadas
                clienteSmtp.Credentials = new NetworkCredential(remitente, contraseña); // Asigna las credenciales del remitente

                using (MailMessage mensaje = new MailMessage(remitente, destinatario))
                {
                    mensaje.Subject = "Código de verificación"; // Asigna el asunto del mensaje
                    mensaje.Body = $"Tu código de verificación es: {codigo}"; // Asigna el cuerpo del mensaje

                    // Envía el mensaje de correo
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
    }
}

