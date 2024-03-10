using System;
using System.Collections.Generic;
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

        public RestCon(int userId)
        {
            InitializeComponent();
            usuarioId = userId;
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            string nuevaContra = txtNueva.Password;
            string confirmarContra = txtConfirmar.Password;

            if (string.IsNullOrEmpty(nuevaContra) || string.IsNullOrEmpty(confirmarContra))
            {
                MessageBox.Show("Por favor, ingrese y confirme la nueva contraseña.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (nuevaContra != confirmarContra)
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                string connectionString = "Data Source=DESKTOP-KIBLMD6\\SQLEXPRESS;Initial Catalog=TelecomunicacionesBD;Integrated Security=true";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Inicio_Sesión SET Contraseña = @NuevaContra WHERE ID_Usuario = @Usuario";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NuevaContra", nuevaContra);
                        command.Parameters.AddWithValue("@Usuario", usuarioId);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                            return;
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar la contraseña. No se encontró el usuario o la contraseña no cumplía con los requisitos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}



