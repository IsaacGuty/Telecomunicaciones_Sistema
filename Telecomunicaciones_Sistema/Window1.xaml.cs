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
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Btn_Registro_Click(object sender, RoutedEventArgs e)
        {
            Window2 formularioD = new Window2();

            formularioD.Show();

            this.Hide();
        }

        private void Btn_Pago(object sender, RoutedEventArgs e)
        {
            Window3 formularioD = new Window3();

            formularioD.Show();

            this.Hide();
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

            MainWindow frmAn = new MainWindow();

            frmAn.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window4 formularioO = new Window4();

            formularioO.Show();

            this.Hide();
        }

        private void BtnEmpleados_Click(object sender, RoutedEventArgs e)
        {
            Empleadosxaml formularioO = new Empleadosxaml();

            formularioO.Show();

            this.Hide();
        }
    }
}
