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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    public partial class Inicio_Sesión : Window
    {
        private Login Objlog = new Login(); // Objeto de tipo Login para gestionar credenciales del usuario
        private Pantalla ObjPan = new Pantalla(); // Objeto de tipo Pantalla para interactuar con la base de datos

        // Variables estáticas para almacenar información del usuario durante el inicio de sesión
        public static string Usuario_L;  // Nombre completo del usuario
        public static string Contraseña_L;  // Contraseña del usuario
        public static string Rol_L;  // Rol del usuario en el sistema
        private int userId; // ID del usuario actual
        private int usuario; // Información del usuario (posiblemente ID u otro tipo de dato)
        public static int IdUsuario; // ID del usuario para acceso global

        private static int intentosI = 0; // Contador de intentos de inicio de sesión fallidos
        private const int maxiI = 3; // Número máximo de intentos permitidos
        private const int duracionI = 5; // Duración del bloqueo en minutos
        private const string archivobloqueo = "bloqueo.txt"; // Nombre del archivo para almacenar la hora de bloqueo

        // Constructor de la clase Inicio_Sesión
        public Inicio_Sesión()
        {
            InitializeComponent(); // Inicializa los componentes de la ventana

            RevisarBloqueo(); // Verifica el estado de bloqueo al iniciar la aplicación
        }

        // Método para manejar el proceso de inicio de sesión
        void P_Login()
        {
            // Comprueba si el usuario está bloqueado
            if (UsuarioBloqueado(out TimeSpan remainingTime))
            {
                // Muestra un mensaje al usuario informando sobre el tiempo de espera necesario
                MessageBox.Show($"Debes esperar {remainingTime.Minutes} minutos y {remainingTime.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Sale del método para evitar nuevos intentos de inicio de sesión
            }

            // Obtiene los datos de usuario de la base de datos usando el objeto Pantalla
            DataTable DT = ObjPan.Pan_Users(new Login { usuario = txtUsuario.Text, contraseña = txtContra.Password });

            if (DT.Rows.Count > 0)
            {
                string usuarioBD = DT.Rows[0]["ID_Empleado"].ToString(); // ID del usuario desde la base de datos
                string contraseñaBD = DT.Rows[0]["Contraseña"].ToString(); // Contraseña desde la base de datos

                // Verifica si los datos ingresados coinciden con los de la base de datos
                if (txtUsuario.Text == usuarioBD && txtContra.Password == contraseñaBD)
                {
                    // Inicio de sesión exitoso
                    intentosI = 0; // Reinicia el contador de intentos fallidos

                    // Muestra un mensaje de bienvenida al usuario
                    MessageBox.Show("Bienvenido(a) " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(),
                                    "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Almacena la información del usuario para su uso posterior
                    Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString();
                    Contraseña_L = contraseñaBD;
                    Rol_L = DT.Rows[0][4].ToString();
                    IdUsuario = Convert.ToInt32(DT.Rows[0]["ID_Empleado"]);

                    // Muestra la ventana del menú y oculta la ventana actual
                    Menú ObjPrinci = new Menú(Usuario_L, Contraseña_L);
                    ObjPrinci.Show();
                    this.Hide();
                }
                else
                {
                    // Maneja el fallo de inicio de sesión
                    ManejarIntentoFallido();
                }
            }
            else
            {
                // Maneja el fallo de inicio de sesión cuando no se encuentra el usuario
                ManejarIntentoFallido();
            }
        }

        // Método para manejar intentos fallidos de inicio de sesión
        void ManejarIntentoFallido()
        {
            intentosI++; // Incrementa el contador de intentos fallidos

            // Limpia los campos de usuario y contraseña
            txtUsuario.Clear();
            txtContra.Clear();

            // Verifica si se ha alcanzado el número máximo de intentos fallidos
            if (intentosI >= maxiI)
            {
                // Establece el bloqueo al usuario
                EstablecerBloqueo();

                // Muestra un mensaje indicando que se ha excedido el número máximo de intentos
                MessageBox.Show("Has excedido el número máximo de intentos de inicio de sesión. Por favor, intenta nuevamente más tarde.",
                                "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

                this.Close(); // Cierra la aplicación
            }
            else
            {
                // Muestra un mensaje indicando que el usuario o la contraseña son incorrectos
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Controlador del evento de clic en el botón de ingreso
        private void Ingresar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si ambos campos de usuario y contraseña están vacíos
            if (Validaciones.CamposVacios(txtUsuario.Text, txtContra.Password))
            {
                // Muestra un mensaje indicando que ambos campos deben ser completados
                MessageBox.Show("Por favor, ingresa tanto el usuario como la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Validaciones.UsuarioVacio(txtUsuario.Text))
            {
                // Muestra un mensaje indicando que el campo de usuario está vacío
                MessageBox.Show("Por favor, ingresa el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Validaciones.ContraseñaVacia(txtContra.Password))
            {
                // Muestra un mensaje indicando que el campo de contraseña está vacío
                MessageBox.Show("Por favor, ingresa la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Validaciones.ContieneSoloNumeros(txtUsuario.Text))
            {
                // Muestra un mensaje indicando que el usuario debe contener solo números
                MessageBox.Show("El usuario debe contener solo números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Llama al método P_Login para intentar iniciar sesión
                P_Login();
            }
        }

        // Controlador del evento de entrada de texto en el campo de usuario
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

        // Controlador del evento de presionar una tecla en el campo de usuario
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

        // Controlador del evento de presionar una tecla en el campo de contraseña
        private void txtContraseña_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            // Valida la entrada de teclas en el campo de contraseña
            if (!Validaciones.ValidarContraseñaEspLong(passwordBox.Password, e, out string mensajeError))
            {
                e.Handled = true; // Marca el evento como manejado para evitar la entrada incorrecta

                // Muestra un mensaje de error al usuario
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Bloquea controles si es necesario
            Validaciones.BloquearControles(e);
        }

        // Controlador del evento de clic en la etiqueta para cambiar la contraseña
        private void lblContraC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide(); // Oculta la ventana actual
            CambContra cmbCon = new CambContra(usuario); // Crea una nueva instancia de la ventana de cambio de contraseña
            cmbCon.Closed += (s, args) => CloseInicio_Sesión(); // Cierra la ventana de inicio de sesión al cerrar la ventana de cambio de contraseña
            cmbCon.ShowDialog(); // Muestra la ventana de cambio de contraseña
        }

        // Método para cerrar la ventana de inicio de sesión
        private void CloseInicio_Sesión()
        {
            this.Close(); // Cierra la ventana Inicio_Sesión
        }

        // Guarda la hora actual en el archivo de bloqueo para establecer el inicio del período de bloqueo
        private void EstablecerBloqueo()
        {
            DateTime TiempoBloqueo = DateTime.Now; // Obtiene la hora actual
            File.WriteAllText(archivobloqueo, TiempoBloqueo.ToString("o")); // Guarda la hora en formato ISO 8601
        }

        // Verifica si el usuario está actualmente bloqueado
        private bool UsuarioBloqueado(out TimeSpan restanteTiempo)
        {
            restanteTiempo = TimeSpan.Zero; // Inicializa el tiempo restante como cero

            // Verifica si existe el archivo de bloqueo
            if (File.Exists(archivobloqueo))
            {
                string cadenaTiempo = File.ReadAllText(archivobloqueo); // Lee el contenido del archivo de bloqueo

                // Intenta convertir la hora de bloqueo leída a un objeto DateTime
                if (DateTime.TryParse(cadenaTiempo, out DateTime bloqueoTiempo))
                {
                    TimeSpan duracionBloqueo = DateTime.Now - bloqueoTiempo; // Calcula cuánto tiempo ha pasado desde el bloqueo
                    restanteTiempo = TimeSpan.FromMinutes(duracionI) - duracionBloqueo; // Calcula el tiempo restante del bloqueo

                    // Verifica si el tiempo restante es positivo
                    if (restanteTiempo.TotalSeconds > 0)
                    {
                        return true; // El bloqueo sigue vigente
                    }
                    else
                    {
                        // El bloqueo ha expirado, elimina el archivo de bloqueo
                        File.Delete(archivobloqueo);
                    }
                }
            }
            return false; // No hay bloqueo activo
        }

        // Verifica el estado del bloqueo al iniciar la aplicación
        private void RevisarBloqueo()
        {
            // Verifica si el usuario está bloqueado para iniciar sesión y, en caso afirmativo, muestra un mensaje de espera
            if (UsuarioBloqueado(out TimeSpan restanteTiempo))
            {
                // Muestra un mensaje al usuario sobre el tiempo de espera necesario
                MessageBox.Show($"Debes esperar {restanteTiempo.Minutes} minutos y {restanteTiempo.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);

                this.Close(); // Cierra la aplicación porque el usuario está bloqueado
            }
        }
    }
}
