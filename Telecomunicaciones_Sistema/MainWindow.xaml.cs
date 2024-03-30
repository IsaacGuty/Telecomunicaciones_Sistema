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
    public partial class MainWindow : Window
    {

        Login Objlog = new Login();
        Pantalla ObjPan = new Pantalla();

        public static String Usuario_L;
        public static String Contraseña_L;
        public static String Rol_L;
        private int userId;
        private int usuario;

        // Método para realizar el inicio de sesión
        void P_Login()
        {
            DataTable DT = new DataTable();
            Objlog.usuario = txtUsuario.Text;
            Objlog.contraseña = txtContra.Password;

            DT = ObjPan.Pan_Users(Objlog);

            if (DT.Rows.Count > 0)
            {
                // Verifica si el usuario y la contraseña coinciden exactamente con los almacenados en la base de datos
                string usuarioBD = DT.Rows[0]["ID_Usuario"].ToString();
                string contraseñaBD = DT.Rows[0]["Contraseña"].ToString();

                if (Objlog.usuario == usuarioBD && Objlog.contraseña == contraseñaBD)
                {
                    MessageBox.Show("Bienvenido " + DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString(), "Mensaje",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    Usuario_L = DT.Rows[0][2].ToString() + " " + DT.Rows[0][3].ToString();
                    Contraseña_L = contraseñaBD; // Almacena la contraseña de la base de datos
                    Rol_L = DT.Rows[0][4].ToString();

                    Window1 ObjPrinci = new Window1(Usuario_L, Contraseña_L);
                    ObjPrinci.Show();

                    txtUsuario.Clear();
                    txtContra.Clear();

                    this.Hide(); // Oculta MainWindow después de mostrar Window1
                }
                else
                {
                    MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtUsuario.Clear();
                    txtContra.Clear();
                }
            }
            else
            {
                MessageBox.Show("Usuario o Contraseña Incorrecta", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Information);
                txtUsuario.Clear();
                txtContra.Clear();
            }
            return;
        }


        // Constructor de la clase MainWindow
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
                P_Login(); // Llama al método para realizar el inicio de sesión
            }
        }

        private void lblContraO_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide(); // Oculta MainWindow antes de mostrar EstContra

            EstContra restCon = new EstContra(usuario);
            restCon.Closed += (s, args) => CloseMainWindow(); // Cierra MainWindow cuando se cierre EstContra
            restCon.ShowDialog();
        }

        private void lblContraC_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Hide();  // Oculta MainWindow antes de mostrar CambContra

            CambContra cmbCon = new CambContra(usuario);
            cmbCon.Closed += (s, args) => CloseMainWindow(); // Cierra MainWindow cuando se cierre CambContra
            cmbCon.ShowDialog();
        }

        // Método para cerrar la ventana MainWindow
        private void CloseMainWindow()
        {
            this.Close(); // Cierra la ventana principal si está abierta
        }
    }
}

