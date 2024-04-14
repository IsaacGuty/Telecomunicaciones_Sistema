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
                MessageBox.Show($"Debes esperar {remainingTime.Minutes} minutos y {remainingTime.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Sale de la función para evitar intentos de inicio de sesión
            }

            DataTable DT = new DataTable();
            Objlog.usuario = txtUsuario.Text;
            Objlog.contraseña = txtContra.Password;

            DT = ObjPan.Pan_Users(Objlog);

            if (DT.Rows.Count > 0)
            {
                // Verifica si el usuario y la contraseña coinciden exactamente con los almacenados en la base de datos
                string usuarioBD = DT.Rows[0]["ID_Usuario"].ToString();
                string contraseñaBD = DT.Rows[0]["Contraseña"].ToString();

                if (Objlog.usuario == usuarioBD && Objlog.contraseña == contraseñaBD)
                {
                    // Restablece el contador de intentos en caso de inicio de sesión exitoso
                    intentosI = 0;

                    MessageBox.Show("Bienvenido " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(),
                                    "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);

                    Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString();
                    Contraseña_L = contraseñaBD; // Almacena la contraseña de la base de datos
                    Rol_L = DT.Rows[0][4].ToString();

                    IdUsuario = Convert.ToInt32(DT.Rows[0]["ID_Usuario"]);

                    Window1 ObjPrinci = new Window1(Usuario_L, Contraseña_L);
                    ObjPrinci.Show();

                    txtUsuario.Clear();
                    txtContra.Clear();

                    this.Hide(); // Oculta MainWindow después de mostrar Window1
                }
                else
                {
                    // Incrementa el contador de intentos
                    intentosI++;

                    MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtUsuario.Clear();
                    txtContra.Clear();

                    // Verifica si ha alcanzado el máximo de intentos
                    if (intentosI >= maxiI)
                    {
                        // Guarda la hora actual en el archivo de bloqueo
                        SetLockout();

                        MessageBox.Show("Has excedido el número máximo de intentos de inicio de sesión. Por favor, intenta nuevamente más tarde.",
                                        "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                        // Cierra la aplicación
                        this.Close();
                    }
                }
            }
            else
            {
                // Incrementa el contador de intentos en caso de usuario no encontrado
                intentosI++;

                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                txtUsuario.Clear();
                txtContra.Clear();

                // Verifica si ha alcanzado el máximo de intentos
                if (intentosI >= maxiI)
                {
                    // Guarda la hora actual en el archivo de bloqueo
                    SetLockout();

                    MessageBox.Show("Has excedido el número máximo de intentos de inicio de sesión. Por favor, intenta nuevamente más tarde.",
                                    "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Cierra la aplicación
                    this.Close();
                }
            }

            return;
        }

        // Evento que se dispara al hacer clic en el botón de inicio de sesión
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Validaciones.ValidarUsuarioYContraseña(txtUsuario.Text, txtContra.Password))
            {
                MessageBox.Show("Por favor, ingresa tanto el usuario como la contraseña.", "Advertencia",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (!Validaciones.ContieneSoloNumeros(txtUsuario.Text))
            {
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                P_Login(); // Llama al método para realizar el inicio de sesión
            }
        }

        // Eventos para restablecer y cambiar la contraseña
        private void lblContraO_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide(); // Oculta MainWindow antes de mostrar EstContra

            EstContra restCon = new EstContra(usuario);
            restCon.Closed += (s, args) => CloseMainWindow(); // Cierra MainWindow cuando se cierre EstContra
            restCon.ShowDialog();
        }

        private void lblContraC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();  // Oculta MainWindow antes de mostrar CambContra

            CambContra cmbCon = new CambContra(usuario);
            cmbCon.Closed += (s, args) => CloseMainWindow(); // Cierra MainWindow cuando se cierre CambContra
            cmbCon.ShowDialog();
        }

        // Método para cerrar la ventana MainWindow
        private void CloseMainWindow()
        {
            this.Close(); // Cierra la ventana principal si está abierta
        }

        private void SetLockout()
        {
            DateTime lockoutTime = DateTime.Now;
            File.WriteAllText(archivobloqueo, lockoutTime.ToString("o"));
        }

        private bool UsuarioBloqueado(out TimeSpan remainingTime)
        {
            remainingTime = TimeSpan.Zero;

            // Verifica si existe el archivo de bloqueo
            if (File.Exists(archivobloqueo))
            {
                // Lee la hora de bloqueo del archivo
                string lockoutTimeString = File.ReadAllText(archivobloqueo);
                if (DateTime.TryParse(lockoutTimeString, out DateTime lockoutTime))
                {
                    // Calcula la duración del bloqueo
                    TimeSpan lockoutDuration = DateTime.Now - lockoutTime;
                    remainingTime = TimeSpan.FromMinutes(duracionI) - lockoutDuration;

                    // Verifica si el bloqueo sigue vigente
                    if (remainingTime.TotalSeconds > 0)
                    {
                        // El bloqueo sigue vigente
                        return true;
                    }
                    else
                    {
                        // El bloqueo ha expirado, elimina el archivo
                        File.Delete(archivobloqueo);
                    }
                }
            }
            // El usuario no está bloqueado
            return false;
        }

        private void RevisarBloqueo()
        {
            if (UsuarioBloqueado(out TimeSpan remainingTime))
            {
                MessageBox.Show($"Debes esperar {remainingTime.Minutes} minutos y {remainingTime.Seconds} segundos para intentar iniciar sesión nuevamente.",
                                "Bloqueo de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Cierra la aplicación
                this.Close();
            }
        }
    }
}

