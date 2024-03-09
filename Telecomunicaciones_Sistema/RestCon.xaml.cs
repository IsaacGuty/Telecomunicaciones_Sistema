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

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para RestCon.xaml
    /// </summary>
    public partial class RestCon : Window
    {
        public RestCon()
        {
            InitializeComponent();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            string nuevaContra = txtNueva.Password;
            string confirmarContra = txtConfirmar.Password;

            if (nuevaContra == confirmarContra)
            {
                MessageBox.Show("¡La contraseña se ha cambiado exitosamente!", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Cierra la ventana de RestCon después de cambiar la contraseña
            }
            else
            {
                MessageBox.Show("Las contraseñas no coinciden. Por favor, inténtalo de nuevo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
