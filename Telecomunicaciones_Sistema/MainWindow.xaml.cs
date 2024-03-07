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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Login Objlog = new Login();
        Pantalla ObjPan = new Pantalla();

        public static String Usuario_L;
        public static String Contraseña_L;
        public static String Rol_L;

        void P_Login()
        {
            DataTable DT = new DataTable();
            Objlog.usuario = txtUsuario.Text;
            Objlog.contraseña = txtContra.Password;

            DT = ObjPan.Pan_Users(Objlog);

            if (DT.Rows.Count > 0)
            {
                MessageBox.Show("Bienvenido " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(), "Mensaje",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString();
                Contraseña_L = DT.Rows[0][1].ToString();    
                Rol_L = DT.Rows[0][4].ToString();

                Window1 ObjPrinci = new Window1(Usuario_L, Contraseña_L);
                ObjPrinci.Show();

                txtUsuario.Clear();
                txtContra.Clear();

                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                txtUsuario.Clear();
                txtContra.Clear();
            }
            return;
        }

            public MainWindow()
            {
                 InitializeComponent();
            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContra.Password))
            {
                MessageBox.Show("Por favor, ingresa tanto el usuario como la contraseña.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                P_Login();
            }
        }
    }
}
