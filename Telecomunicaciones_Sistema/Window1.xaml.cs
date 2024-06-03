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
using System.Diagnostics;

namespace Telecomunicaciones_Sistema
{
    public partial class Window1 : Window
    {
        private bool isMainWindow;

        // Constructor que recibe el usuario y la contraseña (no se usan en este código)
        public Window1(string usuario, string contraseña)
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            Loaded += Window1_Loaded; // Asigna un controlador de eventos al evento Loaded de la ventana
        }

        // Constructor que indica si esta ventana es la ventana principal
        public Window1(bool isMainWindow = false)
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            this.isMainWindow = isMainWindow; // Establece si es la ventana principal
            Loaded += Window1_Loaded; // Asigna un controlador de eventos al evento Loaded de la ventana
        }

        // Controlador de eventos que se ejecuta cuando la ventana se carga
        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            // Muestra el usuario y el rol en las etiquetas correspondientes
            lblUsuario.Content = MainWindow.Usuario_L;
            lblCargo.Content = MainWindow.Rol_L;

            string rol = MainWindow.Rol_L;

            // Habilita o deshabilita los botones según el rol del usuario
            btnRegistro.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsContadora(rol) || Validaciones.IsGerenteTecnico(rol);
            btnPago.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol);
            Btn_OrT.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsTecnico(rol) || Validaciones.IsGerenteTecnico(rol);
            BtnEmpleados.IsEnabled = Validaciones.IsGerenteGeneral(rol);
        }

        private void Btn_Registro_Click(object sender, RoutedEventArgs e)
        {
            // Abre una nueva ventana (Window2) para el formulario de registro
            Window2 formularioD = new Window2();
            formularioD.Show(); // Muestra la ventana de registro
            this.Hide(); // Oculta la ventana actual
        }

        private void Btn_Pago(object sender, RoutedEventArgs e)
        {
            // Abre una nueva ventana (Window3) para el formulario de pago
            Window3 formularioD = new Window3();
            formularioD.Show(); // Muestra la ventana de pago
            this.Hide(); // Oculta la ventana actual
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            // Cierra la ventana actual
            this.Close();
            // Abre la ventana principal (MainWindow)
            MainWindow frmAn = new MainWindow();
            frmAn.Show();
        }

        // Controlador del evento de clic del botón para mostrar órdenes de trabajo
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Abre una nueva ventana (Window4) para mostrar órdenes de trabajo
            Window4 formularioO = new Window4();
            formularioO.Show(); // Muestra la ventana de órdenes de trabajo
            this.Hide(); // Oculta la ventana actual
        }

        private void BtnEmpleados_Click(object sender, RoutedEventArgs e)
        {
            // Abre una nueva ventana (Window6) para mostrar empleados
            Window6 formularioO = new Window6();
            formularioO.Show(); // Muestra la ventana de empleados
            this.Hide(); // Oculta la ventana actual
        }

        private void BtnSoporteTecnico_Click(object sender, RoutedEventArgs e)
        {
            // Muestra un mensaje de bienvenida al servicio de soporte técnico
            MessageBoxResult result = MessageBox.Show("¡Bienvenido al servicio de soporte técnico!\n\n" +
                "Para mayor información puede contactarse a:\n" +
                "1. Número teléfonico: 9755-1953\n" +
                "2. Correo electrónico: telecomunicacioness.2024@gmail.com\n\n" +
                "¿Desea realizar una acción?",
                "Soporte Técnico", MessageBoxButton.YesNo);

            // Si el usuario elige realizar una acción, muestra el panel de acción
            if (result == MessageBoxResult.Yes)
            {
                actionPanel.Visibility = Visibility.Visible;
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            int action;
            // Intenta analizar la entrada del usuario como un número entero
            if (int.TryParse(actionInput.Text, out action))
            {
                // Realiza una acción según el número ingresado por el usuario
                switch (action)
                {
                    case 1:
                        // Abre el enlace de WhatsApp
                        System.Diagnostics.Process.Start("https://api.whatsapp.com/send?phone=97551953");
                        break;
                    case 2:
                        // Abre el correo electrónico
                        System.Diagnostics.Process.Start("mailto:telecomunicaciones_2024@gmail.com");
                        break;
                    default:
                        // Muestra un mensaje indicando que la opción seleccionada no es válida
                        MessageBox.Show("Por favor, seleccione una opción válida.");
                        break;
                }
            }
            else
            {
                // Muestra un mensaje indicando que la entrada no es un número válido
                MessageBox.Show("Por favor, ingrese un número válido.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Oculta el panel de acción
            actionPanel.Visibility = Visibility.Collapsed;
            // Limpia el contenido del TextBox de entrada
            actionInput.Text = "";
        }

        private void ActionInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verificar si el texto completo, incluyendo el carácter que se está ingresando,
            // contiene más de un dígito
            if ((textBox.Text + e.Text).Length > 1)
            {
                // Mostrar mensaje de advertencia
                MessageBox.Show("Solo se permite un dígito.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Bloquear la entrada
                e.Handled = true;
                return; // Salir del método para evitar que se ejecute la siguiente validación
            }

            // Verificar si el texto de entrada es una letra
            if (char.IsLetter(e.Text, e.Text.Length - 1))
            {
                // Mostrar mensaje de error y bloquear la entrada
                MessageBox.Show("No se aceptan letras.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return; // Salir del método para evitar que se ejecute la siguiente validación
            }

            // Verificar si el texto de entrada es un carácter especial
            if (!char.IsLetterOrDigit(e.Text, e.Text.Length - 1))
            {
                // Mostrar mensaje de error y bloquear la entrada
                MessageBox.Show("No se aceptan caracteres especiales.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                return; // Salir del método para evitar que se ejecute la siguiente validación
            }
        }

        private void ActionInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Verificar si se presiona la tecla de espacio
            if (e.Key == Key.Space)
            {
                // Mostrar mensaje de advertencia
                MessageBox.Show("Los espacios en blanco no están permitidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Bloquear la entrada
                e.Handled = true;
            }
        }
    }
}