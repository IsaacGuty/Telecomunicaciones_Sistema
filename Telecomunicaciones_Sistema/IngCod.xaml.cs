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
        private string codRec; // Almacena el código de verificación recibido
        private string correoDestino; // Almacena la dirección de correo electrónico del usuario
        private int userId; // Almacena el ID del usuario
        private string usuario; // Almacena el nombre de usuario
        private readonly Random random = new Random(); // Generador de números aleatorios
        private RestCon winRestCon; // Referencia a la ventana RestCon
        private CamCon winCamCon; // Referencia a la ventana CamCon
        private bool esRestablecer; // Indica si se está restableciendo la contraseña o cambiándola

        // Constructor de la ventana IngCod
        public IngCod(string codigo, string correo, string usuario, int userId, bool esRestablecer)
        {
            InitializeComponent();
            codRec = codigo;
            correoDestino = correo;
            this.userId = userId;
            this.usuario = usuario;
            this.esRestablecer = esRestablecer; // Asigna el valor de esRestablecer al campo correspondiente
            InitializeWindowEvents(); // Inicializa los eventos de la ventana
        }

        // Inicializa los eventos de la ventana
        private void InitializeWindowEvents()
        {
            btnAceptar.Click += BtnAceptar_Click; // Asigna el evento Click al botón Aceptar
            btnReeC.Click += BtnReeC_Click; // Asigna el evento Click al botón ReeC
        }

        // Genera un código de verificación aleatorio
        private string GenerarCodigoAleatorio()
        {
            int codigo = random.Next(100000, 999999); // Genera un número aleatorio de 6 dígitos
            return codigo.ToString(); // Convierte el número aleatorio a una cadena y lo devuelve
        }

        // Envía un correo electrónico con un nuevo código de recuperación de contraseña
        private void EnviarCorreo(string correoDestino, string codigo)
        {
            try
            {
                // Configura el cliente SMTP para enviar correos electrónicos
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("telecomunicacioness.2024@gmail.com", "fast hqaz dejf uxro");

                // Crea un nuevo mensaje de correo electrónico
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("tucorreo@gmail.com"); // Configura el remitente del correo
                mailMessage.To.Add(correoDestino); // Agrega el destinatario del correo
                mailMessage.Subject = "Código"; // Asigna el asunto del correo
                mailMessage.Body = "Tu nuevo código es: " + codigo; // Asigna el cuerpo del correo

                // Envía el correo electrónico
                smtpClient.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Muestra un mensaje de error genérico si ocurre un problema inesperado
                MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si el código ingresado coincide con el código recibido
            if (txtCod.Text == codRec)
            {
                if (esRestablecer)
                {
                    // Abre la ventana para restablecer la contraseña (RestCon)
                    if (winRestCon == null || !winRestCon.IsVisible)
                    {
                        // Muestra un mensaje de confirmación si el código es correcto
                        MessageBox.Show("Código correcto. Ahora puedes restablecer tu contraseña.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Cierra la ventana IngCod antes de abrir RestCon
                        this.Close();

                        winRestCon = new RestCon(userId); // Crea una instancia de RestCon pasando el userId al constructor
                        winRestCon.SetUsuario(usuario); // Establece el nombre de usuario en la ventana RestCon

                        // Muestra la ventana RestCon
                        winRestCon.Show();
                    }
                }
                else
                {
                    // Abre la ventana para cambiar la contraseña (CamCon)
                    if (winCamCon == null || !winCamCon.IsVisible)
                    {
                        // Muestra un mensaje de confirmación si el código es correcto
                        MessageBox.Show("Código correcto. Ahora puedes cambiar tu contraseña.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Cierra la ventana IngCod antes de abrir CamCon
                        this.Close();

                        winCamCon = new CamCon(userId); // Crea una instancia de CamCon pasando el userId al constructor
                        winCamCon.SetUsuario(usuario); // Establece el nombre de usuario en la ventana CamCon

                        // Muestra la ventana CamCon
                        winCamCon.Show();
                    }
                }
            }
            else
            {
                // Muestra un mensaje de error si el código ingresado es incorrecto
                MessageBox.Show("El código ingresado es incorrecto. Por favor, verifica e intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReeC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nuevoCodigoRecuperacion = GenerarCodigoAleatorio();

                codRec = nuevoCodigoRecuperacion;

                EnviarCorreo(correoDestino, nuevoCodigoRecuperacion);

                MessageBox.Show("Se ha vuelto a enviar un código al correo electrónico proporcionado.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reenviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}