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
        private bool isMainWindow;

        public Window1(String usuario, String contraseña)
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

        public Window1()
        {
        }

        private void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            lblUsuario.Content = MainWindow.Usuario_L;
            lblCargo.Content = MainWindow.Rol_L;

            String rol = MainWindow.Rol_L;

            if (rol == "Gerente General")
            {
                btnRegistro.IsEnabled = true;
                btnPago.IsEnabled = true;
                Btn_OrT.IsEnabled = true;
                BtnEmpleados.IsEnabled = true;
            }
            if (rol == "Gerente Tecnico")
            {
                btnRegistro.IsEnabled = false;
                btnPago.IsEnabled = false;
                Btn_OrT.IsEnabled = false;
                BtnEmpleados.IsEnabled = false;
            }
            if (rol == "Tecnico")
            {
                btnRegistro.IsEnabled = false;
                btnPago.IsEnabled = false;
                Btn_OrT.IsEnabled = false;
                BtnEmpleados.IsEnabled = false;
            }
            if (rol == "Secretaria" || rol == "Secretario")
            {
                btnRegistro.IsEnabled = true;
                btnPago.IsEnabled = true;
                Btn_OrT.IsEnabled = true;
                BtnEmpleados.IsEnabled = true;
            }
            if (rol == "Contadora" || rol == "Contador")
            {
                btnRegistro.IsEnabled = false;
                btnPago.IsEnabled = false;
                Btn_OrT.IsEnabled = false;
                BtnEmpleados.IsEnabled = false;
            }
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
