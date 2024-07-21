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
using System.Data.SqlClient;

namespace Telecomunicaciones_Sistema
{
    public partial class CamCon : Window
    {
        // Almacena el ID del usuario
        private int usuarioId;

        // Indica si el usuario ha iniciado sesión (true) o no (false)
        private bool isInicio_Sesión;

        // Almacena el nombre de usuario
        private string usuario;

        // Constructor de la ventana CamCon que acepta el userId como parámetro
        public CamCon(int userId)
        {
            InitializeComponent();
            usuarioId = userId; // Asigna userId a la variable local usuarioId
        }

        // Método para establecer el nombre de usuario en la etiqueta de la ventana
        public void SetUsuario(string ID_Usuario)
        {
            lblusuario.Content = ID_Usuario;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Obtener las contraseñas ingresadas por el usuario
            string anteriorContra = txtAnteriorC.Password;
            string nuevaContra = txtNuevaC.Password;
            string confirmarContra = txtConfirmarC.Password;

            // Validar que todos los campos de contraseña estén completos
            if (!Validaciones.CamposContraseñaCompletos(anteriorContra, nuevaContra, confirmarContra))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar que las nuevas contraseñas coincidan
            if (!Validaciones.ContraseñasCoinciden(nuevaContra, confirmarContra))
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar que la contraseña anterior sea correcta
            if (!InicioDAL.VerificarAntiguaContraseña(lblusuario.Content.ToString(), anteriorContra))
            {
                MessageBox.Show("La contraseña anterior ingresada es incorrecta. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Actualizar la contraseña en la base de datos
                InicioDAL.ActualizarContraseña(lblusuario.Content.ToString(), nuevaContra);

                // Mostrar mensaje de éxito
                MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                Inicio_Sesión formulario = new Inicio_Sesión();
                formulario.Show();

                // Cerrar la ventana actual
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva instancia de la ventana de inicio de sesión.
            Inicio_Sesión Inicio_Sesión = new Inicio_Sesión();

            // Mostrar la ventana de inicio de sesión.
            Inicio_Sesión.Show();

            // Cerrar la ventana actual.
            this.Close();
        }

        private void txtAnteriorC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string textoActual = (sender as PasswordBox).Password + e.Text; // Obtiene el texto combinado

            // Verifica si el texto contiene espacios en blanco
            if (Validaciones.ContieneEspaciosContraseñaA(e.Text))
            {
                MessageBox.Show("No se permiten espacios en blanco en la contraseña anterior.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true; // Cancela la entrada del carácter
            }
            // Verifica si el total de caracteres es mayor que 12
            else if (Validaciones.ExcedeLongitudMaximaContraseñaA(textoActual, 12))
            {
                MessageBox.Show("Se permiten un máximo de 12 dígitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true; // Cancela la entrada del carácter
            }
        }

        private void txtAnteriorC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Verifica si la tecla presionada debe ser bloqueada
            if (Validaciones.TeclaBloqueada(e.Key))
            {
                e.Handled = true; // Cancela la entrada de la tecla
                MessageBox.Show("No se permiten espacios en blanco.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtNuevaC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Intenta convertir el sender al tipo PasswordBox. Este es el control que dispara el evento.
            var passwordBox = sender as PasswordBox;

            // Verifica si la conversión fue exitosa (passwordBox no es null).
            if (passwordBox != null)
            {
                // Declara una variable para almacenar el mensaje de error que se generará en caso de que la validación falle.
                string mensajeError;

                // Llama al método ValidarNuevaContraseña en la clase Validaciones para validar la entrada del texto.
                if (!Validaciones.ValidarNuevaContraseña(e.Text, passwordBox.Password, out mensajeError))
                {
                    // Muestra un mensaje de error al usuario si la validación falla.
                    MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    // Establece e.Handled a true para cancelar la entrada del carácter.
                    e.Handled = true;
                }
            }
        }

        private void txtNuevaC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método de la clase Validaciones para validar que no se introduzcan espacios.
            Validaciones.ValidarEspacios(e);
        }

        private void txtConfirmarC_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // `sender` es el objeto que disparó el evento, en este caso, un PasswordBox. Se convierte a PasswordBox.
            // `e` contiene la información sobre el texto que se está ingresando.

            // Declara una variable para almacenar el mensaje de error, si es necesario.
            string mensajeError;

            // Obtiene el texto actual del PasswordBox y le añade el texto que se va a ingresar.
            string textoActual = (sender as PasswordBox).Password + e.Text;

            // Llama al método `ValidarTextoConfirmacion` de la clase `Validaciones` para verificar si el texto es válido.
            if (!Validaciones.ValidarTextoConfirmacion(textoActual, out mensajeError))
            {
                // Si el texto no es válido, muestra un mensaje de error al usuario con el texto del mensaje de error.
                MessageBox.Show(mensajeError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Cancela la entrada del carácter, evitando que se añada al PasswordBox.
                e.Handled = true;
            }
        }

        private void txtConfirmarC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Llama al método de la clase Validaciones para validar que no se introduzcan espacios.
            Validaciones.ValidarEspacios(e);
        }
    }
}


