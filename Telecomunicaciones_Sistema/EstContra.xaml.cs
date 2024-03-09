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
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace Telecomunicaciones_Sistema
{
    public partial class EstContra : Window
    {
        public EstContra()
        {
            InitializeComponent();
        }

        private bool ExisteUsuario(string idUsuario)
        {
            string connectionString = "Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true";
            string query = "SELECT COUNT(*) FROM Inicio_Sesión WHERE ID_Usuario = @ID_Usuario";

            int count = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                try
                {
                    connection.Open();
                    count = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al verificar la existencia del ID de usuario en la base de datos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return count > 0;
        }

        private string GenerarCodigoAleatorio()
        {
            Random random = new Random();
            int codigo = random.Next(100000, 999999); 
            return codigo.ToString();
        }

        private void EnviarCorreo(string correoDestino, string codigo)
        {
            try
            {
                string nombreRemitente = "Soporte técnico";
                string direccionRemitente = "telecomunicaciones_24@gmail.com";
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("tucorreo@gmail.com", "tucontraseña");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(direccionRemitente, nombreRemitente);
                mailMessage.To.Add(correoDestino);
                mailMessage.Subject = "Código de recuperación de contraseña";
                mailMessage.Body = "Tu código de recuperación de contraseña es: " + codigo;

                smtpClient.Send(mailMessage);

                MessageBox.Show("Se ha enviado un código de recuperación al correo electrónico proporcionado.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtCorreoE.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de enviar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string idUsuario = txtUsuario.Text;
            string correoElectronico = txtCorreoE.Text;

            if (ExisteUsuario(idUsuario))
            {
                string codigoRecuperacion = GenerarCodigoAleatorio();
                EnviarCorreo(correoElectronico, codigoRecuperacion);

                IngCod winCod = new IngCod(codigoRecuperacion, correoElectronico);
                winCod.ShowDialog();
            }
            else
            {
                MessageBox.Show("El usuario especificado no existe en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

