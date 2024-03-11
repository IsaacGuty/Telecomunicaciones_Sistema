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
        // Declaración de eventos para notificar cuando se agrega o modifica un empleado
        public event EventHandler EmpleadoAgregado;
        public event EventHandler EmpleadoModificado;

        // Lista para almacenar empleados
        private List<Empleados> empleados;

        // Propiedad para obtener el nuevo empleado creado
        public Empleados NuevoEmpleado { get; private set; }

        // Variable para indicar si se está modificando un empleado
        private bool esModificacion;

        // Constructor para ventana de modificación/agregación de empleado
        public Window8(bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            // Actualiza la etiqueta según si se está modificando o agregando un empleado
            ActualizarLabel();
        }

        // Constructor para ventana de modificación de empleado con empleado seleccionado
        public Window8(Window6.Empleados empleadoSeleccionado, bool esModificacion)
        {
            InitializeComponent();
            this.esModificacion = esModificacion;
            // Conexión a la base de datos y carga de detalles del empleado seleccionado
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            empleados = new List<Empleados>();
            this.empleadoSeleccionado = empleadoSeleccionado;
            MostrarDetallesEmpleado();
            // Actualiza la etiqueta según si se está modificando o agregando un empleado
            ActualizarLabel();
        }

        // Método para actualizar la etiqueta según si se está modificando o agregando un empleado
        private void ActualizarLabel()
        {
            if (esModificacion)
            {
                lblNom.Content = "Modificar empleado";
            }
            else
            {
                lblNom.Content = "Agregar un nuevo empleado";
            }
        }

        // Constructor sin parámetros (no se utiliza)
        public Window8()
        {
        }

        // Variables de clase
        private SqlConnection Conn;
        private Window6.Empleados empleadoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            // Crea un nuevo objeto Empleados con la información ingresada en los campos
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
                // Verifica si el empleado ya existe en la base de datos
                if (EmpleadoDAL.EmpleadoExiste(NuevoEmpleado.ID_Empleado))
                {
                    // Si el empleado existe, se actualizan sus datos
                    EmpleadoDAL.ActualizarEmpleado(NuevoEmpleado);
                    MessageBox.Show("Empleado modificado correctamente.");
                }
                else
                {
                    // Si el empleado no existe, se agrega como nuevo empleado
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

        // Método para llamar al evento EmpleadoAgregado
        private void OnEmpleadoAgregado()
        {
            EmpleadoAgregado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            // Crea una nueva instancia de Window6 (presumiblemente la ventana principal de empleados) y la muestra
            Window6 frmPr = new Window6();
        }

        // Método para mostrar los detalles del empleado seleccionado en los campos de texto
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

