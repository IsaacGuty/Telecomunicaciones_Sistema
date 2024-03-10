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

namespace Telecomunicaciones_Sistema
{
    public partial class IngCod : Window
    {
        private string codRec;
        private string correoDestino;
        private int userId;
        private readonly Random random = new Random();
        private RestCon ventanaRestCon;

        public IngCod(string codigo, string correo, int userId)
        {
            InitializeComponent();
            codRec = codigo;
            correoDestino = correo;
            this.userId = userId;
            InitializeWindowEvents();
        }

        private void InitializeWindowEvents()
        {
            btnAceptar.Click += BtnAceptar_Click;
            btnReeC.Click += BtnReeC_Click;
        }

        private string GenerarCodigoAleatorio()
        {
            int codigo = random.Next(100000, 999999);
            return codigo.ToString();
        }

        private void EnviarCorreo(string correoDestino, string codigo)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("tucorreo@gmail.com", "tucontraseña");

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("tucorreo@gmail.com");
                mailMessage.To.Add(correoDestino);
                mailMessage.Subject = "Código de recuperación de contraseña";
                mailMessage.Body = "Tu nuevo código de recuperación de contraseña es: " + codigo;

                smtpClient.Send(mailMessage);

                MessageBox.Show("Se ha enviado un código de recuperación actualizado al correo electrónico proporcionado.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show("Error al enviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            if (txtCod.Text == codRec)
            {
                MessageBox.Show("Código correcto. Ahora puedes restablecer tu contraseña.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);

                if (ventanaRestCon == null || !ventanaRestCon.IsVisible) // Verificar si la ventana RestCon ya está abierta
                {
                    ventanaRestCon = new RestCon(userId); // Crear una nueva instancia solo si no existe
                    ventanaRestCon.Closed += (s, args) => this.Show(); // Mostrar esta ventana cuando RestCon se cierre
                    ventanaRestCon.Show();
                    this.Hide(); // Ocultar esta ventana mientras RestCon está abierta
                }
            }
            else
            {
                MessageBox.Show("El código ingresado es incorrecto. Por favor, verifica e intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReeC_Click(object sender, RoutedEventArgs e)
        {
            string nuevoCodigoRecuperacion = GenerarCodigoAleatorio();
            EnviarCorreo(correoDestino, nuevoCodigoRecuperacion);
            MessageBox.Show("Se ha vuelto a enviar un código de recuperación al correo electrónico proporcionado.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

