
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
using System.Text.RegularExpressions;

namespace Telecomunicaciones_Sistema
{
    public partial class IngCod : Window
    {
        private string codRec; // Almacena el código de verificación recibido
        private string correoDestino; // Almacena la dirección de correo electrónico del usuario
        private int userId; // Almacena el ID del usuario
        private string usuario; // Almacena el nombre de usuario
        private readonly Random random = new Random(); // Generador de números aleatorios
        private CamCon winCamCon; // Referencia a la ventana CamCon
        private DateTime codigoGeneradoTime; // Almacenar la fecha y hora de generación del código

        private bool codigoCorrectoMostrado = false; // Variable para controlar si el mensaje de código correcto ha sido mostrado
        private bool codigoIncorrectoMostrado = false; // Variable para controlar si el mensaje de código incorrecto ha sido mostrado
        private bool codigoCampoVacioMostrado = false; // Variable para controlar si el mensaje de campo vacío ha sido mostrado
        private bool codigoEnviadoMostrado = false; // Variable para controlar si se ha mostrado el mensaje de código enviado

        // Constructor de la ventana IngCod
        public IngCod(string codigo, string correo, string usuario, int userId, bool esRestablecer)
        {
            InitializeComponent();
            codRec = codigo;
            correoDestino = correo;
            this.userId = userId;
            this.usuario = usuario;
            InitializeWindowEvents(); // Inicializa los eventos de la ventana
            codigoGeneradoTime = DateTime.Now; // Almacena la fecha y hora actual al momento de generar el código
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
            // Verifica si el campo de texto del código está vacío
            if (string.IsNullOrWhiteSpace(txtCod.Text))
            {
                if (!codigoCampoVacioMostrado)
                {
                    MessageBox.Show("Debes llenar el campo del código.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    codigoCampoVacioMostrado = true; 
                }
                return;
            }

            // Verifica si el código ingresado coincide con el código recibido
            if (txtCod.Text == codRec)
            {
                // Calcula la diferencia de tiempo entre el momento actual y el momento de generación del código
                TimeSpan diferenciaTiempo = DateTime.Now - codigoGeneradoTime;

                // Define el tiempo de vida útil del código en minutos 
                int tiempoVidaCodigo = 5;

                if (diferenciaTiempo.TotalMinutes > tiempoVidaCodigo)
                {
                    // Muestra un mensaje de error si el código ha caducado
                    MessageBox.Show("El código ha caducado. Por favor, solicita un nuevo código.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Muestra el mensaje de código correcto 
                if (!codigoCorrectoMostrado)
                {
                    MessageBox.Show("Código correcto. Ahora puedes cambiar tu contraseña.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                    codigoCorrectoMostrado = true; // Establece la bandera como true para evitar mostrar el mensaje nuevamente
                }

                // Cierra la ventana IngCod antes de abrir CamCon
                this.Close();

                if (winCamCon == null)
                {
                    winCamCon = new CamCon(userId); // Crea una instancia de CamCon pasando el userId al constructor
                }
                winCamCon.SetUsuario(usuario); // Establece el nombre de usuario en la ventana CamCon

                // Muestra la ventana CamCon
                winCamCon.Show();
            }
            else
            {
                // Muestra un mensaje de error si el código ingresado es incorrecto y si no se ha mostrado el mensaje de código correcto
                if (!codigoCorrectoMostrado)
                {
                    MessageBox.Show("El código ingresado es incorrecto. Por favor, verifica e intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    codigoCorrectoMostrado = true;
                }
            }
        }

        private void BtnReeC_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Genera un nuevo código de recuperación aleatorio
                string nuevoCodigoRecuperacion = GenerarCodigoAleatorio();

                // Almacena el nuevo código de recuperación
                codRec = nuevoCodigoRecuperacion;

                // Envía un correo electrónico con el nuevo código de recuperación al correo destino
                EnviarCorreo(correoDestino, nuevoCodigoRecuperacion);

                // Muestra el mensaje de confirmación solo si no se ha mostrado antes
                if (!codigoEnviadoMostrado)
                {
                    MessageBox.Show("Se ha vuelto a enviar un código al correo electrónico proporcionado.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                    codigoEnviadoMostrado = true; // Establece la bandera como true para evitar mostrar el mensaje nuevamente
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al reenviar el correo electrónico: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Cerrar la ventana actual
            this.Close();

            // Mostrar Inicio_Sesión
            Inicio_Sesión Inicio_Sesión = new Inicio_Sesión();
            Inicio_Sesión.Show();
        }

        private void txtCod_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Regex que permite sólo dígitos
            Regex regex = new Regex("[^0-9]+");

            if (txtCod.Text.Length >= 6)
            {
                e.Handled = true;
                MessageBox.Show("Solo se permite un máximo de 6 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (regex.IsMatch(e.Text))
            {
                e.Handled = true;
                if (Regex.IsMatch(e.Text, "[a-zA-Z]"))
                {
                    MessageBox.Show("No se permiten letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("No se permiten caracteres especiales.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtCod_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Si se presiona la barra espaciadora, cancela la entrada
            if (e.Key == Key.Space)
            {
                e.Handled = true;
                MessageBox.Show("No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}