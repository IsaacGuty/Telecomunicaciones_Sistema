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
using System.Text.RegularExpressions;
using System.Globalization;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        private SqlConnection Conn;

        public Window4()
        {
            InitializeComponent();
            // Conexión a la base de datos
            Conn = BD.ObtenerConexion();
            // Al inicializar la ventana, carga los datos en el DataGrid
            CargarDatos();

            CargarEmpleadosTecnicos();
        }

        // Propiedad para almacenar los datos de la orden
        public Ordenes DatosOrden { get; set; }

        // Variable para identificar si esta ventana es la ventana principal
        private bool isMainWindow;

        // Método para cargar los datos en el DataGrid al iniciar la ventana
        private void CargarDatos()
        {
            try
            {
                // Obtiene los datos de las órdenes y los muestra en el DataGrid
                DataTable dataTable = OrdenDAL.ObtenerOrdenes();
                DatGridOT.ItemsSource = dataTable.DefaultView;
                DatGridOT.IsReadOnly = true; // Establecer el DataGrid como solo lectura
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void CargarEmpleadosTecnicos()
        {
            try
            {
                // Obtener los empleados técnicos
                DataTable dataTable = EmpleadoDAL.ObtenerEmpleadosTecnicos();
                // Limpiar ComboBox cmbNombreE
                cmbNombreE.Items.Clear();
                // Agregar empleados técnicos al ComboBox cmbNombreE
                foreach (DataRow row in dataTable.Rows)
                {
                    string nombreCompleto = row["Nombre_E"].ToString() + " " + row["Apellido_E"].ToString();
                    cmbNombreE.Items.Add(nombreCompleto);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los empleados técnicos: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de la ventana principal y la muestra
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            // Cierra esta ventana si no es la ventana principal
            if (!isMainWindow)
            {
                this.Close();
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Implementación para imprimir
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpia los campos de búsqueda
            txtNombre.Clear();
            txtApellido.Clear();
            txtDirección.Clear();
            txtNumT.Clear();
            txtTpServicio.Clear();
            txtBuscar.Clear();

            // Restablece el placeholder y color del campo de búsqueda
            txtBuscar.Text = "Nombre, apellido";
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);

            // Recarga los datos en el DataGrid
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Realizar la validación del texto de búsqueda
            if (!Validaciones.BusquedaOValida(txtBuscar.Text, out string mensaje))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show(mensaje, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Imprimir el criterio de búsqueda
            Console.WriteLine("Criterio de búsqueda: " + txtBuscar.Text);

            // Obtener los datos de las órdenes que coinciden con el criterio de búsqueda y mostrarlos en el DataGrid
            DataTable dataTable = OrdenDAL.BuscarOrden(txtBuscar.Text);
            DataView dataView = dataTable.DefaultView;
            DatGridOT.ItemsSource = dataView;

            // Verificar si la búsqueda no devolvió ningún resultado después de mostrar los datos
            if (dataTable.Rows.Count == 0)
            {
                // Mostrar mensaje indicando que no se encontró ninguna orden que coincida con el criterio de búsqueda
                MessageBox.Show("No se encontró ninguna orden que coincida con el criterio de búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Evento que se ejecuta cuando se selecciona una fila en el DataGrid
        private void DatGridOT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Muestra los detalles de la orden seleccionada en los campos correspondientes
            if (DatGridOT.SelectedItem != null)
            {
                DataRowView rowView = DatGridOT.SelectedItem as DataRowView;

                if (rowView != null)
                {
                    txtNombre.Text = rowView["Nombre"].ToString();
                    txtApellido.Text = rowView["Apellido"].ToString();
                    txtDirección.Text = rowView["Dirección"].ToString();
                    txtNumT.Text = rowView["Teléfono"].ToString();
                    txtTpServicio.Text = rowView["Servicio"].ToString();
                }
            }
        }

        // Variables para almacenar la selección en los ComboBox
        private string valorSeleccionadoTipoT;
        private string valorSeleccionadoNombreE;

        // Evento que se ejecuta al cambiar la selección en el ComboBox de Tipo de Trabajo
        private void CmbTipoT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener el tipo de trabajo seleccionado del ComboBox cmbTipoT
            if (cmbTipoT.SelectedItem != null)
            {
                valorSeleccionadoTipoT = ((ComboBoxItem)cmbTipoT.SelectedItem).Content.ToString();
            }
        }

        // Evento que se ejecuta al cambiar la selección en el ComboBox de Nombre de Empleado
        private void CmbNombreE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualiza la selección en el ComboBox de Tipo de Trabajo en otra ventana
            if (cmbNombreE.SelectedItem != null)
            {
                valorSeleccionadoNombreE = cmbNombreE.SelectedItem.ToString();

                // Obtener el ID del empleado seleccionado
                string idEmpleado = ObtenerIdEmpleadoSeleccionado(valorSeleccionadoNombreE);

                // Crear una instancia de Window5 si no existe una instancia previa
                Window5 ventana5 = Application.Current.Windows.OfType<Window5>().FirstOrDefault();
                if (ventana5 == null)
                {
                    ventana5 = new Window5();
                }

                // Asignar los datos de la orden a la ventana5
                ventana5.DatosOrden = DatosOrden;

                // Actualizar los datos en la ventana5
                ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE, idEmpleado);
            }
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (!Validaciones.CamposOrdenVacios(txtNombre.Text, txtApellido.Text, txtDirección.Text, txtNumT.Text, txtTpServicio.Text, cmbTipoT.SelectedItem, cmbNombreE.SelectedItem))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de imprimir.", "Datos Incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Obtener el ID del empleado seleccionado
                string idEmpleado = ObtenerIdEmpleadoSeleccionado(cmbNombreE.SelectedItem.ToString());

                // Guardar la orden en la base de datos
                OrdenDAL.GuardarOrden(new Ordenes
                {
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Dirección = txtDirección.Text,
                    Teléfono = Convert.ToDecimal(txtNumT.Text),
                    Servicio = txtTpServicio.Text,
                    Tp_Servicio = valorSeleccionadoTipoT,
                    Nombre_E = cmbNombreE.SelectedItem.ToString(),
                    ID_Empleado = idEmpleado
                });

                // Crear una instancia de Window5 si no existe una instancia previa
                Window5 ventana5 = Application.Current.Windows.OfType<Window5>().FirstOrDefault();
                if (ventana5 == null)
                {
                    ventana5 = new Window5();
                }

                // Asignar los datos de la orden a la ventana5
                ventana5.DatosOrden = new Ordenes
                {
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Dirección = txtDirección.Text,
                    Teléfono = Convert.ToDecimal(txtNumT.Text),
                    Servicio = txtTpServicio.Text
                };

                // Actualizar los datos en la ventana5
                ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE, idEmpleado);

                ventana5.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la orden: " + ex.Message);
            }
        }

        // Método para obtener el ID del empleado seleccionado en el ComboBox
        private string ObtenerIdEmpleadoSeleccionado(string nombreCompleto)
        {
            try
            {
                // Buscar el ID del empleado en base al nombre completo
                DataTable dataTable = EmpleadoDAL.BuscarEmpleadoNombreCompleto(nombreCompleto);
                if (dataTable.Rows.Count > 0)
                {
                    // Obtener el ID del primer empleado encontrado
                    return dataTable.Rows[0]["ID_Empleado"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el ID del empleado: " + ex.Message);
            }

            // Si no se pudo encontrar el empleado, devolver null
            return null;
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtBuscar.Text == "Nombre, apellido")
            {
                txtBuscar.Text = "";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Nombre, apellido";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                // Convierte la primera letra de cada palabra a mayúscula
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }

        private void SetPlaceholderText()
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "Nombre, apellido";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
    }
}

