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
                ActualizarContraseña(nuevaContra);

                MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error de SQL al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error inesperado al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Método para actualizar la contraseña en la base de datos
        private void ActualizarContraseña(string nuevaContra)
        {
            // Cadena de conexión a la base de datos
            string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";

            try
            {
                // Abre la conexión a la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Inicio_Sesión SET Contraseña = @NuevaContra WHERE ID_Usuario = @Usuario";

                    // Prepara y ejecuta la consulta SQL para actualizar la contraseña
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevaContra", nuevaContra);
                        command.Parameters.AddWithValue("@Usuario", lblusuario.Content.ToString());

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        // Verifica si se actualizaron filas en la base de datos
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el usuario en la base de datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error de SQL al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error inesperado al cambiar la contraseña: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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





