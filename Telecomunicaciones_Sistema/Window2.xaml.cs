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
    /// Lógica de interacción para Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public bool SeleccionDesdeVentana9 { get; set; }

        private bool SeleccionDesdeVentana10 = false;

        private Window7 ventana7; // Ventana para agregar o modificar clientes

        // Propiedad estática para almacenar el cliente seleccionado
        public Clientes ClienteSeleccionado { get; set; }

        // Constructor de la ventana
        public Window2()
        {
            InitializeComponent();
            // Establecer conexión a la base de datos
            Conn = BD.ObtenerConexion();           
            // Cargar los datos de los clientes en el DataGrid
            CargarDatos();

            // Crear una instancia de Window7
            ventana7 = new Window7();

            // Suscribir al evento ClienteAgregado de Window7 para actualizar los datos en esta ventana
            ventana7.ClienteAgregado += ActualizarDatosCliente;
        }


        // Clase interna para el diálogo de nuevo cliente 
        public partial class NuevoClienteDialog : Window
        {
            // Propiedades del diálogo de nuevo cliente
            public string ID_Cliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Teléfono { get; set; }
            public string Correo { get; set; }
            public string ID_Dirección { get; set; }
        }

        // Estructura para representar un cliente
        public struct Clientes
        {
            public string ID_Cliente;
            public string Nombre;
            public string Apellido;
            public string Teléfono;
            public string Correo;
            public string ID_Dirección;
        }

        private SqlConnection Conn; // Conexión a la base de datos
        private bool isMainWindow; // Indica si esta ventana es la principal o no

        // Método para cargar los datos de los clientes desde la base de datos al DataGrid
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = ClienteDAL.ObtenerTodosClientes(); // Obtener datos de clientes desde la base de datos
                DatGridRC.ItemsSource = dataTable.DefaultView; // Mostrar los datos en el DataGrid
                DatGridRC.IsReadOnly = true; // Establecer el DataGrid como solo lectura
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message); // Mostrar mensaje de error en caso de fallo
            }
        }

        // Manejador de eventos para el cambio de selección en el DataGrid
        private void DatGridRC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridRC.SelectedItem != null && DatGridRC.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridRC.SelectedItem as DataRowView;

                // Almacena los datos del cliente seleccionado en la estructura Clientes
                ClienteSeleccionado = new Clientes
                {
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    Nombre = rowView["Nombre"].ToString(),
                    Apellido = rowView["Apellido"].ToString(),
                    Teléfono = rowView["Teléfono"].ToString(),
                    Correo = rowView["Correo"].ToString(),
                    ID_Dirección = rowView["ID_Dirección"].ToString()
                };
            }
            else
            {
                ClienteSeleccionado = default(Clientes); // Restablecer el cliente seleccionado si no hay selección
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionCliente(false); // Solicitar información de un nuevo cliente
        }

        // Método para solicitar información de un nuevo cliente
        private void SolicitarInformacionCliente(bool esModificacion)
        {
            MessageBoxResult result;
            if (esModificacion)
            {
                result = MessageBox.Show("Por favor, ingrese la información del cliente a modificar.", "Modificar Cliente", MessageBoxButton.OKCancel);
            }
            else
            {
                result = MessageBox.Show("Por favor, ingrese la información del nuevo cliente.", "Nuevo Cliente", MessageBoxButton.OKCancel);
            }

            if (result == MessageBoxResult.OK)
            {
                // Mostrar la ventana7 para agregar o modificar un cliente
                Window7 frmAg = new Window7(esModificacion);
                frmAg.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana 7
                frmAg.Show();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si se ha ingresado un criterio de búsqueda
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un criterio de búsqueda
                MessageBox.Show("Debe ingresar el ID_Cliente, Nombre o Apellido, para realizar la búsqueda de clientes.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un criterio de búsqueda
            }

            // Buscar clientes según el texto ingresado en el campo de búsqueda
            DataTable dataTable = ClienteDAL.BuscarCliente(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DatGridRC.ItemsSource = dataView;

            // Verificar si el DataTable está vacío
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron clientes que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpiar el campo de búsqueda
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray); // Restablecer el color del texto a gris
            txtBuscar.Text = "ID, nombre, apellido"; // Restablecer el placeholder
            CargarDatos(); // Recargar todos los datos de los clientes en el DataGrid
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del cliente.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                if (!ClienteSeleccionado.Equals(default(Clientes)))
                {
                    if (ventana7 == null || !ventana7.IsVisible) // Verifica si la ventana7 ya está abierta
                    {
                        // Abre la ventana7 para modificar el cliente seleccionado
                        ventana7 = new Window7(ClienteSeleccionado, true);
                        ventana7.ClienteModificado += ActualizarDatosCliente;
                        ventana7.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana 7
                        ventana7.Show();
                    }
                    else
                    {
                        ventana7.Activate(); // Muestra la ventana7 si ya está abierta
                    }
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún cliente."); // Mensaje si no hay ningún cliente seleccionado
                }
            }
        }

        // Método para actualizar los datos del cliente en el DataGrid
        private void ActualizarDatosCliente(object sender, EventArgs e)
        {
            // Cargar los datos nuevamente para reflejar el nuevo cliente agregado o modificado
            CargarDatos();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            if (Window10.SeleccionDesdeVentana10)
            {
                // Ocultar la ventana actual y mostrar la ventana 10
                this.Hide();
                Window10 ventana10 = Window10.Instance;
            }
            else if (SeleccionDesdeVentana9)
            {
                // Ocultar la ventana actual y mostrar la ventana 9
                this.Hide();
                Window9 ventana9 = Window9.Instance;
                ventana9.Show();
            }
            else
            {
                // Ocultar la ventana actual y mostrar la ventana 1
                this.Hide();
                Window1 frmPr = new Window1(isMainWindow: true);
                frmPr.Show();

                if (!isMainWindow)
                {
                    this.Close();
                }
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

        private void SetPlaceholderText()
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = "ID, nombre, apellido";
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }
    }
}


