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

namespace Telecomunicaciones_Sistema
{
    public partial class MainWindow : Window
    {
        private Login Objlog = new Login();
        private Pantalla ObjPan = new Pantalla();

        public static string Usuario_L;
        public static string Contraseña_L;
        public static string Rol_L;
        private int userId;
        private int usuario;
        public static int IdUsuario;

        // Contador de intentos de inicio de sesión
        private int intentosI = 0;
        private const int maxiI = 3;
        private const int duracionI = 5; // Duración del bloqueo en minutos

        // Archivo para almacenar la hora de bloqueo
        private const string archivobloqueo = "lockout.txt";

        // Constructor de la clase MainWindow
        public MainWindow()
        {
            InitializeComponent();

            // Verifica el bloqueo al iniciar la aplicación
            RevisarBloqueo();
        }

        // Método para realizar el inicio de sesión
        void P_Login()
        {
            // Comprueba si el usuario está actualmente bloqueado
            if (UsuarioBloqueado(out TimeSpan remainingTime))
            {
                // Muestra un mensaje indicando al usuario que debe esperar un tiempo antes de intentar iniciar sesión nuevamente.
                MessageBox.Show($"Debes esperar {remainingTime.Minutes} minutos y {remainingTime.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Sale de la función para evitar intentos de inicio de sesión
            }

            DataTable DT = new DataTable(); // Crea un objeto DataTable para almacenar los datos de usuario obtenidos de la base de datos.
            Objlog.usuario = txtUsuario.Text; // Asigna el texto de txtUsuario al objeto Login.
            Objlog.contraseña = txtContra.Password; // Asigna la contraseña ingresada al objeto Login.

            DT = ObjPan.Pan_Users(Objlog); // Llama a Pan_Users() para obtener los datos de usuario de la base de datos.

            if (DT.Rows.Count > 0)
            {
                // Si hay al menos una fila en el DataTable (es decir, se encontró un usuario)
                string usuarioBD = DT.Rows[0]["ID_Usuario"].ToString(); // Almacena el ID de usuario desde la base de datos.
                string contraseñaBD = DT.Rows[0]["Contraseña"].ToString(); // Almacena la contraseña desde la base de datos.

                // Verifica si los datos ingresados coinciden con los almacenados en la base de datos
                if (Objlog.usuario == usuarioBD && Objlog.contraseña == contraseñaBD)
                {
                    // Restablece el contador de intentos en caso de inicio de sesión exitoso
                    intentosI = 0;

                    // Muestra un mensaje de bienvenida al usuario
                    MessageBox.Show("Bienvenido(a) " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(),
                                    "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Almacena el nombre de usuario, la contraseña y el rol en variables estáticas para su uso posterior
                    Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString();
                    Contraseña_L = contraseñaBD;
                    Rol_L = DT.Rows[0][4].ToString();

                    // Almacena el ID de usuario en una variable estática para su uso posterior
                    IdUsuario = Convert.ToInt32(DT.Rows[0]["ID_Usuario"]);

                    // Crea una nueva instancia de la ventana Window1 y la muestra
                    Window1 ObjPrinci = new Window1(Usuario_L, Contraseña_L);
                    ObjPrinci.Show();

                    txtUsuario.Clear();
                    txtContra.Clear();

                    this.Hide();
                }
                else
                {
                    // Incrementa el contador de intentos fallidos de inicio de sesión
                    intentosI++;

                    // Muestra un mensaje indicando que el usuario o la contraseña son incorrectos
                    MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Limpia los campos de texto de usuario y contraseña
                    txtUsuario.Clear();
                    txtContra.Clear();

                    // Verifica si se ha alcanzado el número máximo de intentos fallidos
                    if (intentosI >= maxiI)
                    {
                        // Guarda la hora actual en el archivo de bloqueo
                        SetLockout();

                        // Muestra un mensaje indicando que se ha excedido el número máximo de intentos
                        MessageBox.Show("Has excedido el número máximo de intentos de inicio de sesión. Por favor, intenta nuevamente más tarde.",
                                        "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Cierra la aplicación
                        this.Close();
                    }
                }
            }
            else
            {
                // Incrementa el contador de intentos fallidos si no se encontró un usuario
                intentosI++;

                // Muestra un mensaje indicando que el usuario o la contraseña son incorrectos
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);

                // Limpia los campos de texto de usuario y contraseña
                txtUsuario.Clear();
                txtContra.Clear();

                // Verifica si se ha alcanzado el número máximo de intentos fallidos
                if (intentosI >= maxiI)
                {
                    // Guarda la hora actual en el archivo de bloqueo
                    SetLockout();

                    // Muestra un mensaje indicando que se ha excedido el número máximo de intentos
                    MessageBox.Show("Has excedido el número máximo de intentos de inicio de sesión. Por favor, intenta nuevamente más tarde.",
                                    "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Cierra la aplicación
                    this.Close();
                }
            }

            return;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si los campos de usuario y contraseña están vacíos
            if (Validaciones.ValidarUsuarioYContraseña(txtUsuario.Text, txtContra.Password))
            {
                // Muestra un mensaje advirtiendo que ambos campos deben estar completos
                MessageBox.Show("Por favor, ingresa tanto el usuario como la contraseña.", "Advertencia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (!Validaciones.ContieneSoloNumeros(txtUsuario.Text))
            {
                // Muestra un mensaje advirtiendo que el usuario debe contener solo números
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Llama a P_Login() para iniciar sesión
                P_Login();
            }
        }

        private void lblContraO_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();

            EstContra restCon = new EstContra(usuario);
            restCon.Closed += (s, args) => CloseMainWindow();
            restCon.ShowDialog();
        }

        private void lblContraC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();

            CambContra cmbCon = new CambContra(usuario);
            cmbCon.Closed += (s, args) => CloseMainWindow(); 
            cmbCon.ShowDialog();
        }

        private void CloseMainWindow()
        {
            this.Close(); // Cierra la ventana MainWindow
        }

        // Guarda la hora actual como tiempo de bloqueo en el archivo lockout.txt
        private void SetLockout()
        {
            DateTime lockoutTime = DateTime.Now;
            File.WriteAllText(archivobloqueo, lockoutTime.ToString("o"));
        }

        // Verifica si el usuario está bloqueado
        private bool UsuarioBloqueado(out TimeSpan remainingTime)
        {
            // Inicializa el tiempo restante como cero
            remainingTime = TimeSpan.Zero;

            // Verifica si existe el archivo de bloqueo
            if (File.Exists(archivobloqueo))
            {
                // Lee el contenido del archivo lockout.txt, que contiene la hora de bloqueo
                string lockoutTimeString = File.ReadAllText(archivobloqueo);

                // Intenta convertir la hora de bloqueo leída a un objeto DateTime
                if (DateTime.TryParse(lockoutTimeString, out DateTime lockoutTime))
                {
                    // Calcula cuánto tiempo ha pasado desde el bloqueo
                    TimeSpan lockoutDuration = DateTime.Now - lockoutTime;

                    // Calcula el tiempo restante de bloqueo
                    remainingTime = TimeSpan.FromMinutes(duracionI) - lockoutDuration;

                    // Verifica si el tiempo restante es mayor que cero
                    if (remainingTime.TotalSeconds > 0)
                    {
                        // Si el tiempo restante es positivo, el bloqueo sigue vigente
                        return true;
                    }
                    else
                    {
                        // Si el tiempo restante es negativo o cero, el bloqueo ha expirado
                        // Elimina el archivo de bloqueo porque ya no es necesario
                        File.Delete(archivobloqueo);
                    }
                }
            }

            // Si no existe el archivo de bloqueo o el bloqueo ha expirado, el usuario no está bloqueado
            return false;
        }

        // Verifica si el usuario está bloqueado al iniciar la aplicación
        private void RevisarBloqueo()
        {
            // Verifica si el usuario está bloqueado para iniciar sesión, y si es así, calcula el tiempo restante de bloqueo.
            if (UsuarioBloqueado(out TimeSpan remainingTime))
            {
                // Muestra un mensaje al usuario informando cuánto tiempo debe esperar antes de poder intentar iniciar sesión nuevamente.
                MessageBox.Show($"Debes esperar {remainingTime.Minutes} minutos y {remainingTime.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Cierra la aplicación, ya que el usuario está bloqueado y no puede iniciar sesión en este momento.
                this.Close();
            }
        }
    }
}

