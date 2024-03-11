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
        private int usuarioId; // Almacena el ID del usuario
        private bool isMainWindow;
        private string usuario;

        // Constructor de la ventana CamCon que acepta el userId como parámetro
        public CamCon(int userId)
        {
            InitializeComponent();
            usuarioId = userId; // Asigna userId a la variable local usuarioId
        }

        public CamCon(string usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
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
            if (!Validaciones.VerificarAntiguaContraseña(lblusuario.Content.ToString(), anteriorContra))
            {
                MessageBox.Show("La contraseña anterior ingresada es incorrecta. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Actualizar la contraseña en la base de datos
                Validaciones.ActualizarContraseña(lblusuario.Content.ToString(), nuevaContra);

                MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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


