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
    public partial class Window1 : Window
    {
        private bool isMainWindow;

        public Window1(string usuario, string contraseña)
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }

        public Window1(bool isMainWindow = false)
        {
            InitializeComponent();
            this.isMainWindow = isMainWindow;
            Loaded += Window1_Loaded;
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            lblUsuario.Content = MainWindow.Usuario_L;
            lblCargo.Content = MainWindow.Rol_L;

            string rol = MainWindow.Rol_L;

            btnRegistro.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsContadora(rol);
            btnPago.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol);
            Btn_OrT.IsEnabled = Validaciones.IsGerenteGeneral(rol) || Validaciones.IsSecretaria(rol) || Validaciones.IsTecnico(rol);
            BtnEmpleados.IsEnabled = Validaciones.IsGerenteGeneral(rol);
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
            Window6 formularioO = new Window6();
            formularioO.Show();
            this.Hide();
        }
    }
}