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
    public partial class Menú : Window
    {
        private bool isInicio_Sesión;

        // Constructor que recibe el usuario y la contraseña (no se usan en este código)
        public Menú(string usuario, string contraseña)
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            Loaded += Menú_Loaded; // Asigna un controlador de eventos al evento Loaded de la ventana
        }

        // Constructor que indica si esta ventana es la ventana principal
        public Menú(bool isInicio_Sesión = false)
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            this.isInicio_Sesión = isInicio_Sesión; // Establece si es la ventana principal
            Loaded += Menú_Loaded; // Asigna un controlador de eventos al evento Loaded de la ventana
        }

        // Controlador de eventos que se ejecuta cuando la ventana se carga
        private void Menú_Loaded(object sender, RoutedEventArgs e)
        {
            // Muestra el usuario y el rol en las etiquetas correspondientes
            lblUsuario.Content = Inicio_Sesión.Usuario_L;
            lblCargo.Content = Inicio_Sesión.Rol_L;

            string rol = Inicio_Sesión.Rol_L;

            // Habilita o deshabilita los botones según el rol del usuario
            btnRegistro.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsContadora(rol) || Validaciones.IsGerenteTecnico(rol);
            btnPago.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol);
            Btn_OrT.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsTecnico(rol) || Validaciones.IsGerenteTecnico(rol);
            BtnEmpleados.IsEnabled = Validaciones.IsGerenteGeneral(rol);
        }

        private void Btn_Registro_Click(object sender, RoutedEventArgs e)
        {
            Registro_Cliente formularioD = new Registro_Cliente(); // Abre una nueva ventana (Registro_Cliente) para el formulario de registro
            formularioD.Show(); // Muestra la ventana de registro
            this.Hide(); // Oculta la ventana actual
        }

        private void Btn_Pago(object sender, RoutedEventArgs e)
        {
            Registro_Pago formularioD = new Registro_Pago();  // Abre una nueva ventana (Registro_Pago) para el formulario de pago
            formularioD.Show(); // Muestra la ventana de pago
            this.Hide(); // Oculta la ventana actual
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Cierra la ventana actual
            Inicio_Sesión frmAn = new Inicio_Sesión(); // Abre la ventana principal (Inicio_Sesión)
            frmAn.Show();
        }

        // Controlador del evento de clic del botón para mostrar órdenes de trabajo
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Abre una nueva ventana (Registro_Orden) para mostrar órdenes de trabajo
            Registro_Orden formularioO = new Registro_Orden();
            formularioO.Show(); // Muestra la ventana de órdenes de trabajo
            this.Hide(); // Oculta la ventana actual
        }

        private void BtnEmpleados_Click(object sender, RoutedEventArgs e)
        {
            Registro_Empleado formularioO = new Registro_Empleado(); // Abre una nueva ventana (Registro_Empleado) para mostrar empleados
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
                        System.Diagnostics.Process.Start("https://api.whatsapp.com/send?phone=97551953"); // Abre el enlace de WhatsApp
                        break;
                    case 2:
                        System.Diagnostics.Process.Start("mailto:telecomunicaciones_2024@gmail.com"); // Abre el correo electrónico
                        break;
                    default:
                        // Muestra un mensaje indicando que la opción seleccionada no es válida
                        MessageBox.Show("Por favor, ingrese una opción válida (1 o 2).");
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
            actionPanel.Visibility = Visibility.Collapsed; // Oculta el panel de acción
            actionInput.Text = ""; // Limpia el contenido del TextBox de entrada
        }

        private void ActionInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Verificar si el texto completo, incluyendo el carácter que se está ingresando, contiene más de un dígito
            if ((textBox.Text + e.Text).Length > 1)
            {
                // Mostrar mensaje de advertencia
                MessageBox.Show("Solo se permite un dígito.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                e.Handled = true; // Bloquear la entrada
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