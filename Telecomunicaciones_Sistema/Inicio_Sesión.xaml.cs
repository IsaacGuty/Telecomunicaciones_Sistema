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
    public partial class Inicio_Sesión : Window
    {
        private Login Objlog = new Login(); // Define un objeto de tipo Login
        private Pantalla ObjPan = new Pantalla(); // Define un objeto de tipo Pantalla 

        // Variables estáticas para almacenar información de usuario durante el inicio de sesión
        public static string Usuario_L;  // Nombre de usuario
        public static string Contraseña_L;  // Contraseña del usuario
        public static string Rol_L;  // Rol del usuario en el sistema
        private int userId; // Variable para almacenar el ID del usuario actual
        private int usuario; // Variable para almacenar información del usuario (puede ser ID u otro tipo de dato)
        public static int IdUsuario; // Variable estática para almacenar el ID del usuario

        private static int intentosI = 0; // Contador de intentos de inicio de sesión
        private const int maxiI = 3;
        private const int duracionI = 5; // Duración del bloqueo en minutos
        private const string archivobloqueo = "lockout.txt"; // Archivo para almacenar la hora de bloqueo

        // Constructor de la clase Inicio_Sesión
        public Inicio_Sesión()
        {
            InitializeComponent();

            RevisarBloqueo(); // Verifica el bloqueo al iniciar la aplicación
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
                    intentosI = 0; // Restablece el contador de intentos en caso de inicio de sesión exitoso

                    // Muestra un mensaje de bienvenida al usuario
                    MessageBox.Show("Bienvenido(a) " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(),
                                    "Mensaje", MessageBoxButton.OK, MessageBoxImage.Information);

                    Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(); // Almacena el nombre de usuario, la contraseña y el rol en variables estáticas para su uso posterior
                    Contraseña_L = contraseñaBD;
                    Rol_L = DT.Rows[0][4].ToString();

                    IdUsuario = Convert.ToInt32(DT.Rows[0]["ID_Usuario"]); // Almacena el ID de usuario en una variable estática para su uso posterior

                    Menú ObjPrinci = new Menú(Usuario_L, Contraseña_L); // Crea una nueva instancia de la ventana Menú y la muestra
                    ObjPrinci.Show();

                    this.Hide(); // Oculta la ventana actual
                }
                else
                {
                    intentosI++; // Incrementa el contador de intentos fallidos de inicio de sesión

                    // Limpia los campos de usuario y contraseña
                    txtUsuario.Clear();
                    txtContra.Clear();

                    // Verifica si se ha alcanzado el número máximo de intentos fallidos
                    if (intentosI >= maxiI)
                    {
                        SetLockout(); // Guarda la hora actual en el archivo de bloqueo

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
            }
            else
            {
                intentosI++; // Incrementa el contador de intentos fallidos si no se encontró un usuario

                // Limpia los campos de usuario y contraseña
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

                    this.Close(); // Cierra la aplicación
                }
                else
                {
                    // Muestra un mensaje indicando que el usuario o la contraseña son incorrectos
                    MessageBox.Show("Usuario o Contraseña Incorrecta", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return;
        }

        // Controlador del evento de clic del botón para ingresar
        private void Ingresar_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si ambos campos de usuario y contraseña están vacíos
            if (Validaciones.CamposVacios(txtUsuario.Text, txtContra.Password))
            {
                // Muestra un mensaje advirtiendo que ambos campos deben estar completos
                MessageBox.Show("Por favor, ingresa tanto el usuario como la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Validaciones.UsuarioVacio(txtUsuario.Text))
            {
                // Muestra un mensaje advirtiendo que el usuario debe ser ingresado
                MessageBox.Show("Por favor, ingresa el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Validaciones.ContraseñaVacia(txtContra.Password))
            {
                // Muestra un mensaje advirtiendo que la contraseña debe ser ingresada
                MessageBox.Show("Por favor, ingresa la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (!Validaciones.ContieneSoloNumeros(txtUsuario.Text))
            {
                // Muestra un mensaje advirtiendo que el usuario debe contener solo números
                MessageBox.Show("El usuario debe contener solo números.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                P_Login(); // Llama a P_Login() para iniciar sesión
            }
        }

        private void txtUsuario_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si el carácter ingresado es un número
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Si el carácter no es un número, marca el evento como manejado para evitar que se agregue

                // Verifica si el carácter es una letra
                if (char.IsLetter(e.Text, 0))
                {
                    // Muestra un mensaje informativo al usuario
                    MessageBox.Show("No se permiten letras en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // Muestra un mensaje informativo al usuario
                    MessageBox.Show("No se permiten caracteres especiales en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return;
            }

            // Verifica si el texto resultante después de agregar el nuevo carácter excederá la longitud máxima permitida
            if (textBox.Text.Length + e.Text.Length > 7)
            {
                e.Handled = true; // Si excede la longitud máxima permitida, marca el evento como manejado para evitar que se agregue el nuevo carácter

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("El usuario no puede contener más de 7 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtUsuario_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                e.Handled = true; // Marca el evento como manejado para evitar que se agregue el espacio

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en el campo de usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Validaciones.BloquearControles(e);
        }

        private void txtContraseña_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;

            // Verifica si la tecla presionada es la barra espaciadora
            if (e.Key == Key.Space)
            {
                e.Handled = true;  // Marca el evento como manejado para evitar que se agregue el espacio

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("No se permiten espacios en blanco en la contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // Verifica si la longitud de la contraseña supera los 12 caracteres
            else if (passwordBox.Password.Length >= 12 && !char.IsControl((char)KeyInterop.VirtualKeyFromKey(e.Key)))
            {
                e.Handled = true; // Marca el evento como manejado para evitar que se agregue más caracteres

                // Muestra un mensaje informativo al usuario
                MessageBox.Show("La contraseña no puede tener más de 12 caracteres.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Validaciones.BloquearControles(e);
        }

        private void lblContraC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide(); // Ocultar la ventana actual
            CambContra cmbCon = new CambContra(usuario); // Crear una nueva instancia de la ventana de cambio de contraseña
            cmbCon.Closed += (s, args) => CloseInicio_Sesión(); // Asociar un evento para cerrar la ventana principal cuando se cierre la ventana de cambio de contraseña
            cmbCon.ShowDialog(); // Mostrar la ventana de cambio de contraseña
        }

        private void CloseInicio_Sesión()
        {
            this.Close(); // Cierra la ventana Inicio_Sesión
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
            remainingTime = TimeSpan.Zero;  // Inicializa el tiempo restante como cero

            // Verifica si existe el archivo de bloqueo
            if (File.Exists(archivobloqueo))
            {
                string lockoutTimeString = File.ReadAllText(archivobloqueo); // Lee el contenido del archivo lockout.txt, que contiene la hora de bloqueo

                // Intenta convertir la hora de bloqueo leída a un objeto DateTime
                if (DateTime.TryParse(lockoutTimeString, out DateTime lockoutTime))
                {
                    TimeSpan lockoutDuration = DateTime.Now - lockoutTime; // Calcula cuánto tiempo ha pasado desde el bloqueo

                    remainingTime = TimeSpan.FromMinutes(duracionI) - lockoutDuration; // Calcula el tiempo restante de bloqueo

                    // Verifica si el tiempo restante es mayor que cero
                    if (remainingTime.TotalSeconds > 0)
                    {
                        return true; // Si el tiempo restante es positivo, el bloqueo sigue vigente
                    }
                    else
                    {
                        // Si el tiempo restante es negativo o cero, el bloqueo ha expirado
                        // Elimina el archivo de bloqueo porque ya no es necesario
                        File.Delete(archivobloqueo);
                    }
                }
            }
            return false;  // Si no existe el archivo de bloqueo o el bloqueo ha expirado, el usuario no está bloqueado
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

                this.Close(); // Cierra la aplicación, ya que el usuario está bloqueado y no puede iniciar sesión en este momento.
            }
        }
    }
}

