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
using System.Globalization;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Registro_Empleado.xaml
    /// </summary>
    public partial class Registro_Empleado : Window
    {
        private Agregar_Empleado ventana8;

        // Propiedad estática para almacenar el empleado seleccionado
        public static Empleados EmpleadoSeleccionado { get; set; }
        public Modificar_Empleado Modificar_Empleado { get; private set; }
        // Constructor de la ventana
        public Registro_Empleado()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion();
            CargarDatos();
            ventana8 = new Agregar_Empleado();

            ventana8.EmpleadoAgregado += ActualizarDatosEmpleado; // Suscribir al evento ClienteAgregado de Agregar_Cliente para actualizar los datos en esta ventana
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

        private SqlConnection Conn; // Conexión a la base de datos
        private bool isInicio_Sesión;

        // Método para cargar los datos de los empleados desde la base de datos al DataGrid
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = EmpleadoDAL.ObtenerTodosEmpleados();
                DataGridEMP.ItemsSource = dataTable.DefaultView;
                DataGridEMP.IsReadOnly = true; // Establecer el DataGrid como solo lectura

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Manejador de eventos para el botón de regresar
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            // Cerrar la ventana actual si no es la ventana principal
            if (!isInicio_Sesión)
            {
                this.Close();
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo empleado.", "Nuevo Empleado", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Mostrar la ventana para agregar un nuevo empleado
                Agregar_Empleado frmAg = new Agregar_Empleado();
                frmAg.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar el cuadro de búsqueda y recargar los datos de los empleados
            txtBuscar.Clear();
            txtBuscar.Text = "ID, nombre, apellido";
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);

            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar la validación del texto de búsqueda
            if (!Validaciones.BusquedaEValida(txtBuscar.Text, out string mensaje))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Imprimir el criterio de búsqueda para depuración
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Realizar una búsqueda de empleado según el texto ingresado en el cuadro de búsqueda
            DataTable dataTable = EmpleadoDAL.BuscarEmpleado(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DataGridEMP.ItemsSource = dataView;

            // Verificar si el DataTable está vacío
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron empleados que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del empleado.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                if (!EmpleadoSeleccionado.Equals(default(Empleados)))
                {
                    // Crear una nueva ventana para modificar el cliente seleccionado
                    Modificar_Empleado = new Modificar_Empleado(EmpleadoSeleccionado, true);
                    Modificar_Empleado.EmpleadoModificado += ActualizarDatosEmpleado;
                    Modificar_Empleado.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana 8
                    Modificar_Empleado.Show();
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún empleado."); // Mensaje si no hay ningún empleado seleccionado
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

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "ID, nombre, apellido")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "ID, nombre, apellido";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                // Convierte la primera letra de cada palabra a mayúscula
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }
    }
}