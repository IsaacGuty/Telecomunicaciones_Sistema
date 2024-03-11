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
        // Dirección de correo electrónico del remitente
        private readonly string remitente = "telecomunicacioness.2024@gmail.com";
        // Contraseña del remitente
        private readonly string contraseña = "fast hqaz dejf uxro";
        // ID del usuario
        private int userId;

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
                // Genera un código de verificación
                int codigo = GenerarCodVerif();

                // Verifica que se hayan completado los campos de usuario y correo electrónico
                if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtCorreoE.Text))
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

                // Envía un correo electrónico con el código de verificación
                EnviarCorreo(correo, codigo);

                // Registra la actividad de envío de código de verificación en un archivo de registro
                RegistrarActividad("Envío de código de verificación", usuario, correo);

                MessageBox.Show($"Se ha enviado un código de verificación al correo electrónico asociado al usuario.", "Código de verificación", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cierra la ventana actual y abre la ventana para ingresar el código de verificación
                this.Close();
                IngCod winCod = new IngCod(codigo.ToString(), correo, usuario, userId, false);
                winCod.ShowDialog();
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
    }
}

