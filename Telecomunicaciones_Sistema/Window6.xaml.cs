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
        // Propiedad estática para almacenar el empleado seleccionado
        public static Empleados EmpleadoSeleccionado { get; set; }

        // Conexión a la base de datos
        SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
        private bool isMainWindow;

        // Constructor de la ventana
        public Window6()
        {
            InitializeComponent();
            // Cargar los datos de los empleados al iniciar la ventana
            CargarDatos();
        }

        // Estructura para representar un empleado
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

        // Método para cargar los datos de los empleados desde la base de datos al DataGrid
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = EmpleadoDAL.ObtenerTodosEmpleados();
                DataGridEMP.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Manejador de eventos para el botón de regresar
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            // Cerrar la ventana actual si no es la ventana principal
            if (!isMainWindow)
            {
                this.Close();
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana de agregar empleado
            SolicitarInformacionEmpleado(false);
        }

        // Método para solicitar información de un nuevo empleado
        private void SolicitarInformacionEmpleado(bool esModificacion)
        {
            // Mostrar un mensaje para solicitar la información del nuevo empleado
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo empleado.", "Nuevo Empleado", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Abrir la ventana de agregar/modificar empleado
                Window8 frmAg = new Window8(esModificacion);
                frmAg.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar el cuadro de búsqueda y recargar los datos de los empleados
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar una búsqueda de empleado según el texto ingresado en el cuadro de búsqueda
            DataTable dataTable = EmpleadoDAL.BuscarEmpleado(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DataGridEMP.ItemsSource = dataView;
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            // Mostrar un mensaje para solicitar la modificación del empleado seleccionado
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del empleado.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Verificar si hay un empleado seleccionado
                if (!EmpleadoSeleccionado.Equals(default(Empleados)))
                {
                    // Abrir la ventana de modificar empleado y pasar el empleado seleccionado
                    Window8 frmMd = new Window8(EmpleadoSeleccionado, true);
                    frmMd.EmpleadoModificado += ActualizarDatosEmpleado; // Suscribirse al evento de modificación de empleado
                    frmMd.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana
                    frmMd.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún empleado.");
                }
            }
        }

        // Método para actualizar los datos del empleado en el DataGrid después de una modificación
        private void ActualizarDatosEmpleado(object sender, EventArgs e)
        {
            CargarDatos();
        }

        // Manejador de eventos para el cambio de selección en el DataGrid
        private void DataGridEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el empleado seleccionado en el DataGrid
            if (DataGridEMP.SelectedItem != null && DataGridEMP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DataGridEMP.SelectedItem as DataRowView;

                EmpleadoSeleccionado = new Empleados
                {
                    ID_Empleado = rowView["ID_Empleado"].ToString(),
                    Nombre_E = rowView["Nombre_E"].ToString(),
                    Apellido_E = rowView["Apellido_E"].ToString(),
                    Teléfono_E = rowView["Teléfono_E"].ToString(),
                    Correo_E = rowView["Correo_E"].ToString(),
                    ID_Dirección = rowView["ID_Dirección"].ToString(),
                    Puesto = rowView["Puesto"].ToString(),
                    Estado = rowView["Estado"].ToString()
                };
            }
            else
            {
                EmpleadoSeleccionado = default(Empleados);
            }
        }
    }
}

