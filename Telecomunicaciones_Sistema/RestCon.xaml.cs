using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace Telecomunicaciones_Sistema
{
    public partial class RestCon : Window
    {
        private int usuarioId;
        private bool isMainWindow;

        // Constructor de la ventana RestCon que acepta el userId como parámetro
        public RestCon(int userId)
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
            string nuevaContra = txtNueva.Password;
            string confirmarContra = txtConfirmar.Password;

            // Validar si los campos de contraseña están vacíos
            if (Validaciones.CamposContraseñaVacios(nuevaContra, confirmarContra))
            {
                MessageBox.Show("Por favor, ingrese y confirme la nueva contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validar si las contraseñas no coinciden
            if (Validaciones.ContraseñasNoCoinciden(nuevaContra, confirmarContra))
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Intenta actualizar la contraseña
                string usuario = lblusuario.Content.ToString(); // Obtener el nombre de usuario desde el control lblusuario
                Validaciones.ActualizarContraseñaa(usuario, nuevaContra); // Llamar a la validación pasando el nombre de usuario y la nueva contraseña como parámetros

                // Mostrar mensaje de éxito
                MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow formulario = new MainWindow();
                formulario.Show();

                // Cerrar la ventana actual
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }
    }
}





