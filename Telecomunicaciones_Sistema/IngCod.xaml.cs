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
        private string usuario;
        private readonly Random random = new Random();
        private RestCon winRestCon;
        private CamCon winCamCon;
        private bool esRestablecer;

        public IngCod(string codigo, string correo, string usuario, int userId)
        {
            InitializeComponent();
            codRec = codigo;
            correoDestino = correo;
            this.userId = userId;
            this.usuario = usuario;
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

                if (esRestablecer)
                {
                    // Abre la ventana para restablecer la contraseña (RestCon)
                    if (winRestCon == null || !winRestCon.IsVisible)
                    {
                        winRestCon = new RestCon(userId); // Pasar userId al constructor de RestCon
                        winRestCon.SetUsuario(usuario); // Pasar el valor de usuario a RestCon
                        winRestCon.Closed += (s, args) => this.Show();
                        winRestCon.Show();
                        this.Hide();
                    }
                }
                else
                {
                    // Abre la ventana para cambiar la contraseña (CamCon)
                    if (winCamCon == null || !winCamCon.IsVisible)
                    {
                        winCamCon = new CamCon(userId); // Pasar userId al constructor de CamCon
                        winCamCon.SetUsuario(usuario); // Pasar el valor de usuario a CamCon
                        winCamCon.Closed += (s, args) => this.Show();
                        winCamCon.Show();
                        this.Hide();
                    }
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

