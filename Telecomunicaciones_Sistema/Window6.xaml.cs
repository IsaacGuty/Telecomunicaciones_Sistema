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
using System.Data;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Window6.xaml
    /// </summary>
    public partial class Window6 : Window
    {

        SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");

        public Window6()
        {
            InitializeComponent();

            CargarDatos();
        }

        public partial class NuevoEmpleadoDialog : Window
        {
            public string ID_Empleado { get; set; }
            public string Nombre_E { get; set; }
            public string Apellido_E { get; set; }
            public string Teléfono_E { get; set; }
            public string Correo_E { get; set; }
            public string ID_Dirección { get; set; }
            public string Puesto { get; set; }
            public string Estado { get; set; }
        }

        public struct Empleados
        {
            public string ID_Empleado;
            public string Nombre_E;
            public string Apellido_E;
            public string Teléfono_E;
            public string Correo_E;
            public string ID_Dirección;
            public string Puesto;
            public string Estado;
        }

        private void CargarDatos()
        {
            try
            {
                Conn.Open();
                string query = "SELECT * FROM Empleados";
                SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "Empleados");
                DataGridEMP.ItemsSource = dataSet.Tables["Empleados"].DefaultView;
                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window1 frmPr = new Window1();

            frmPr.Show();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionEmpleado();
        }

        private void SolicitarInformacionEmpleado()
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo empleado.", "Nuevo Empleado", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Window8 frmAg = new Window8();
                frmAg.Closed += (s, args) => CargarDatos();
                frmAg.Show();
            }

        }
    }
}
