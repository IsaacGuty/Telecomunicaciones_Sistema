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
    /// Lógica de interacción para Window8.xaml
    /// </summary>
    public partial class Window8 : Window
    {
        public event EventHandler EmpleadoAgregado;

        private List<Empleados> empleados;

        public Empleados NuevoEmpleado { get; private set; }


        public Window8()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
        }

        private SqlConnection Conn;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            NuevoEmpleado = new Empleados
            {
                ID_Empleado = txtIDE.Text,
                Nombre_E = txtNombreE.Text,
                Apellido_E = txtApellidoE.Text,
                Teléfono_E = Convert.ToDecimal(txtTelefonoE.Text),
                Correo_E = txtCorreoE.Text,
                ID_Dirección = txtDireccionE.Text,
                Puesto = txtPuesto.Text,
                Estado = txtEstado.Text
            };
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Empleados (ID_Empleado, Nombre_E, Apellido_E, Teléfono_E, Correo_E, ID_Dirección, Puesto, Estado) VALUES (@ID_Empleado, @Nombre_E, @Apellido_E, @Teléfono_E, @Correo_E, @ID_Dirección, @Puesto, @Estado)", Conn);
                cmd.Parameters.AddWithValue("@ID_Empleado", NuevoEmpleado.ID_Empleado);
                cmd.Parameters.AddWithValue("@Nombre_E", NuevoEmpleado.Nombre_E);
                cmd.Parameters.AddWithValue("@Apellido_E", NuevoEmpleado.Apellido_E);
                cmd.Parameters.AddWithValue("@Teléfono_E", NuevoEmpleado.Teléfono_E);
                cmd.Parameters.AddWithValue("@Correo_E", NuevoEmpleado.Correo_E);
                cmd.Parameters.AddWithValue("@ID_Dirección", NuevoEmpleado.ID_Dirección);
                cmd.Parameters.AddWithValue("@Puesto", NuevoEmpleado.Puesto);
                cmd.Parameters.AddWithValue("@Estado", NuevoEmpleado.Estado);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Empleado agregado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el empleado: " + ex.Message);
            }
            OnEmpleadoAgregado();

            this.Close();
        }

        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }
    }
}
