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
        private readonly string remitente = "telecomunicacioness.2024@gmail.com";
        private readonly string contraseña = "fast hqaz dejf uxro";
        private int userId;

        public EstContra(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Inicializa userId con el valor proporcionado
        }

        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int codigo = GenerarCodVerif();

                if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtCorreoE.Text))
                {
                    MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string usuario = txtUsuario.Text;
                string correo = txtCorreoE.Text;

                if (!CorreoValido(correo))
                {
                    MessageBox.Show("Por favor, ingrese un correo electrónico válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!UsuarioExiste(usuario))
                {
                    MessageBox.Show("El usuario proporcionado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                EnviarCorreo(correo, codigo);
                RegistrarActividad("Envío de código de verificación", usuario, correo);

                MessageBox.Show($"Se ha enviado un código de verificación al correo electrónico asociado al usuario.", "Código de verificación", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
                IngCod winCod = new IngCod(codigo.ToString(), correo, userId);
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

        private int GenerarCodVerif()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999);
        }

        private void RegistrarActividad(string actividad, string usuario, string correo)
        {
            string logMessage = $"{DateTime.Now}: {actividad} - Usuario: {usuario}, Correo: {correo}";
            string filePath = "log.txt";
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        }

        private bool CorreoValido(string correo)
        {
            string patron = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(correo, patron);
        }

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

        private bool UsuarioExiste(string usuario)
        {
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Inicio_Sesión WHERE ID_Usuario = @Usuario";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario", usuario);

                    connection.Open();

                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }
    }
}