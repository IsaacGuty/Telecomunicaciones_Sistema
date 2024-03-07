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
    /// Lógica de interacción para Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        public Window5()
        {
            InitializeComponent();
        }

        private bool isMainWindow;

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
