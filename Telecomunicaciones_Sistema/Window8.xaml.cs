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

        public event EventHandler EmpleadoModificado;

        private List<Empleados> empleados;

        public Empleados NuevoEmpleado { get; private set; }

        private bool esModificacion;

        public Window8(bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente";
            }
        }

        public Window8(Window6.Empleados empleadoSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
            this.empleadoSeleccionado = empleadoSeleccionado;
            MostrarDetallesEmpleado();
            ActualizarLabel();
        }

        private void ActualizarLabel()
        {
            if (esModificacion)
            {
                lblNom.Content = "Modificar cliente";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo cliente";
            }
        }

        public Window8(bool esModificacion, bool esOtraModificacion)
        {
            InitializeComponent();
            if (esOtraModificacion)
            {
                // Haz algo con esOtraModificacion si es necesario
            }
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
        }

        public Window8()
        {
        }

        private SqlConnection Conn;
        private Window6.Empleados empleadoSeleccionado;

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
                if (EmpleadoDAL.EmpleadoExiste(NuevoEmpleado.ID_Empleado))
                {
                    // Empleado existente, actualiza los datos
                    EmpleadoDAL.ActualizarEmpleado(NuevoEmpleado);
                    MessageBox.Show("Empleado modificado correctamente.");
                }
                else
                {
                    // Empleado no existente, agrega un nuevo empleado
                    EmpleadoDAL.AgregarEmpleado(NuevoEmpleado);
                    MessageBox.Show("Empleado agregado correctamente.");

                    // Llama al evento EmpleadoAgregado antes de cerrar la ventana
                    OnEmpleadoAgregado();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar/agregar el empleado: " + ex.Message);
            }

            // Cierra la ventana después de procesar el empleado
            this.Close();
        }


        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window6 frmPr = new Window6();
        }

        private void GuardarCambios()
        {

        }

        private void OnEmpleadoModificado()
        {
            EmpleadoModificado?.Invoke(this, EventArgs.Empty);
        }

        private void MostrarDetallesEmpleado()
        {
            txtIDE.Text = empleadoSeleccionado.ID_Empleado;
            txtNombreE.Text = empleadoSeleccionado.Nombre_E;
            txtApellidoE.Text = empleadoSeleccionado.Apellido_E;
            txtCorreoE.Text = empleadoSeleccionado.Correo_E;
            txtTelefonoE.Text = empleadoSeleccionado.Teléfono_E;
            txtDireccionE.Text = empleadoSeleccionado.ID_Dirección;
            txtPuesto.Text = empleadoSeleccionado.Puesto;
            txtEstado.Text = empleadoSeleccionado.Estado;
        }
    }
}
