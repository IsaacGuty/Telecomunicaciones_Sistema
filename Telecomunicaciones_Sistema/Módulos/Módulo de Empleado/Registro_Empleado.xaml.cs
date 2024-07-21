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
        // Variable para almacenar una instancia de la ventana de agregar empleado
        private Agregar_Empleado ventana8;

        // Propiedad estática que almacena el empleado actualmente seleccionado
        public static Empleados EmpleadoSeleccionado { get; set; }

        // Propiedad para la ventana de modificación de empleado
        public Modificar_Empleado Modificar_Empleado { get; private set; }

        // Constructor de la ventana de registro de empleado
        public Registro_Empleado()
        {
            InitializeComponent(); // Inicializa los componentes de la ventana
            Conn = BD.ObtenerConexion(); // Obtiene la conexión a la base de datos
            CargarDatos(); // Carga los datos de los empleados al DataGrid
            ventana8 = new Agregar_Empleado(); // Inicializa la ventana de agregar empleado

            // Suscribe al evento EmpleadoAgregado de la ventana Agregar_Empleado para actualizar los datos cuando se agregue un nuevo empleado
            ventana8.EmpleadoAgregado += ActualizarDatosEmpleado;
        }

        // Estructura para representar la información de un empleado
        public struct Empleados
        {
            public string ID_Empleado;
            public string Nombre_E;
            public string Apellido_E;
            public string Teléfono_E;
            public string Correo_E;
            public string ID_Dirección;
            public string Puesto;
            public string ID_Estado;
        }

        private SqlConnection Conn; // Conexión a la base de datos
        private bool isInicio_Sesión; // Indica si estamos en la sesión de inicio

        // Método para cargar los datos de los empleados desde la base de datos al DataGrid
        private void CargarDatos()
        {
            try
            {
                // Obtiene todos los empleados desde la base de datos
                DataTable dataTable = EmpleadoDAL.ObtenerTodosEmpleados();
                DataGridEMP.ItemsSource = dataTable.DefaultView; // Asigna los datos al DataGrid
                DataGridEMP.IsReadOnly = true; // Establece el DataGrid como solo lectura
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si ocurre una excepción
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Manejador de eventos para el botón de regresar
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia del menú principal y la muestra
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            // Cierra la ventana actual si no es la ventana de inicio de sesión
            if (!isInicio_Sesión)
            {
                this.Close();
            }
        }

        // Manejador de eventos para el botón de agregar empleado
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Muestra un mensaje solicitando la información del nuevo empleado
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo empleado.", "Nuevo Empleado", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Muestra la ventana para agregar un nuevo empleado
                Agregar_Empleado frmAg = new Agregar_Empleado();
                frmAg.Closed += (s, args) => CargarDatos(); // Refresca los datos del DataGrid cuando se cierre la ventana
                frmAg.Show();
            }
        }

        // Manejador de eventos para el botón de limpiar búsqueda
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpia el cuadro de búsqueda y restablece el texto y el color del texto
            txtBuscar.Clear();
            txtBuscar.Text = "ID, nombre, apellido";
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);

            // Vuelve a cargar los datos de los empleados
            CargarDatos();
        }

        // Manejador de eventos para el botón de búsqueda
        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realiza la validación del texto de búsqueda
            if (!Validaciones.BusquedaEValida(txtBuscar.Text, out string mensaje))
            {
                // Muestra un mensaje advirtiendo al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detiene la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Imprime el criterio de búsqueda para depuración
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Realiza una búsqueda de empleado según el texto ingresado en el cuadro de búsqueda
            DataTable dataTable = EmpleadoDAL.BuscarEmpleado(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DataGridEMP.ItemsSource = dataView;

            // Verifica si el DataTable está vacío
            if (dataTable.Rows.Count == 0)
            {
                // Muestra un mensaje si no se encontraron empleados que coincidan con la búsqueda
                MessageBox.Show("No se encontraron empleados que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Manejador de eventos para el botón de modificar empleado
        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            // Muestra un mensaje solicitando la modificación del empleado
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del empleado.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Verifica si hay un empleado seleccionado
                if (!EmpleadoSeleccionado.Equals(default(Empleados)))
                {
                    // Crea una nueva ventana para modificar el empleado seleccionado
                    Modificar_Empleado = new Modificar_Empleado(EmpleadoSeleccionado, true);
                    Modificar_Empleado.EmpleadoModificado += ActualizarDatosEmpleado; // Suscribe al evento EmpleadoModificado para actualizar los datos
                    Modificar_Empleado.Closed += (s, args) => CargarDatos(); // Refresca los datos del DataGrid cuando se cierre la ventana de modificación
                    Modificar_Empleado.Show();
                }
                else
                {
                    // Muestra un mensaje si no se ha seleccionado ningún empleado
                    MessageBox.Show("No se ha seleccionado ningún empleado.");
                }
            }
        }

        // Método para actualizar los datos del empleado en el DataGrid después de una modificación
        private void ActualizarDatosEmpleado(object sender, EventArgs e)
        {
            CargarDatos(); // Vuelve a cargar los datos de los empleados
        }

        // Manejador de eventos para el cambio de selección en el DataGrid
        private void DataGridEMP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verifica si el DataGrid tiene un elemento seleccionado
            if (DataGridEMP.SelectedItem != null && DataGridEMP.SelectedItem is DataRowView)
            {
                // Obtiene la vista de la fila seleccionada
                DataRowView rowView = DataGridEMP.SelectedItem as DataRowView;

                // Asigna la información del empleado seleccionado a la propiedad EmpleadoSeleccionado
                EmpleadoSeleccionado = new Empleados
                {
                    ID_Empleado = rowView["ID_Empleado"].ToString(),
                    Nombre_E = rowView["Nombre_E"].ToString(),
                    Apellido_E = rowView["Apellido_E"].ToString(),
                    Teléfono_E = rowView["Teléfono_E"].ToString(),
                    Correo_E = rowView["Correo_E"].ToString(),
                    ID_Dirección = rowView["ID_Dirección"].ToString(),
                    Puesto = rowView["Puesto"].ToString(),
                    ID_Estado = rowView["ID_Estado"].ToString()
                };
            }
            else
            {
                // Restablece EmpleadoSeleccionado a su valor predeterminado si no hay ningún empleado seleccionado
                EmpleadoSeleccionado = default(Empleados);
            }
        }

        // Manejador de eventos para cuando el cuadro de búsqueda recibe el enfoque
        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            // Limpia el texto de búsqueda y cambia el color a negro cuando el cuadro recibe el enfoque
            if (txtBuscar.Text == "ID, nombre, apellido")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        // Manejador de eventos para cuando el cuadro de búsqueda pierde el enfoque
        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            // Restaura el texto de búsqueda y el color si el cuadro está vacío
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
