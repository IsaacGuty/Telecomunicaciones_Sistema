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
    /// Lógica de interacción para Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();

            // Al inicializar la ventana, carga los datos en el DataGrid
            CargarDatos();
        }

        // Propiedad para almacenar los datos de la orden
        public Ordenes DatosOrden { get; set; }

        // Conexión a la base de datos
        SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
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
            cmbTipoT.Items.Clear();
            cmbNombreE.Items.Clear();
            txtBuscar.Clear();

            // Recarga los datos en el DataGrid
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                CargarDatos();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Obtiene los datos de las órdenes que coinciden con el criterio de búsqueda y los muestra en el DataGrid
            DataTable dataTable = OrdenDAL.BuscarOrden(txtBuscar.Text);
            DataView dataView = dataTable.DefaultView;
            DatGridOT.ItemsSource = dataView;
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
            // Actualiza la selección en el ComboBox de Nombre de Empleado en otra ventana
            if (cmbTipoT.SelectedItem != null)
            {
                ListBoxItem itemSeleccionado = cmbTipoT.SelectedItem as ListBoxItem;
                if (itemSeleccionado != null)
                {
                    valorSeleccionadoTipoT = itemSeleccionado.Content.ToString();

                    Window5 ventana5 = new Window5();
                    ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE);
                }
            }
        }

        // Evento que se ejecuta al cambiar la selección en el ComboBox de Nombre de Empleado
        private void CmbNombreE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualiza la selección en el ComboBox de Tipo de Trabajo en otra ventana
            if (cmbNombreE.SelectedItem != null)
            {
                ListBoxItem itemSeleccionado = cmbNombreE.SelectedItem as ListBoxItem;
                if (itemSeleccionado != null)
                {
                    valorSeleccionadoNombreE = itemSeleccionado.Content.ToString();

                    Window5 ventana5 = new Window5();
                    ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE);
                }
            }
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) || string.IsNullOrEmpty(txtDirección.Text) || string.IsNullOrEmpty(txtNumT.Text) || string.IsNullOrEmpty(txtTpServicio.Text) || cmbTipoT.SelectedItem == null || cmbNombreE.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos antes de imprimir.", "Datos Incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window5 ventana5 = new Window5();

            // Asigna los datos de la orden a la nueva ventana
            ventana5.DatosOrden = new Ordenes
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Dirección = txtDirección.Text,
                Teléfono = Convert.ToDecimal(txtNumT.Text),
                Servicio = txtTpServicio.Text
            };

            // Actualiza los datos en la nueva ventana
            ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE);

            ventana5.Show();
            this.Hide();
        }
    }
}

