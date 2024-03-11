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
    /// Lógica de interacción para Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        private Window7 ventana7;

        // Propiedad estática para almacenar el cliente seleccionado
        public static Clientes ClienteSeleccionado { get; set; }

        // Constructor de la ventana
        public Window2()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            CargarDatos();

            // Crear una instancia de Window7
            ventana7 = new Window7();

            // Suscribirte al evento ClienteAgregado
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

        // Variables de clase
        private SqlConnection Conn;
        private bool isMainWindow;

        // Método para cargar los datos de los clientes
        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = ClienteDAL.ObtenerTodosClientes();
                DatGridRC.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }

        // Manejador de eventos para el cambio de selección en el DataGrid
        private void DatGridRC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridRC.SelectedItem != null && DatGridRC.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridRC.SelectedItem as DataRowView;

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
                ClienteSeleccionado = default(Clientes);
            }
        }

        // Método para inicialización de la ventana
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionCliente(false);
            
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
                // Pasar información sobre si es una modificación o no a Window7
                Window7 frmAg = new Window7(esModificacion);
                frmAg.Closed += (s, args) => CargarDatos(); // Refrescar los datos del DataGrid cuando se cierre la ventana 7
                frmAg.Show();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            DataTable dataTable = ClienteDAL.BuscarCliente(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DatGridRC.ItemsSource = dataView;
        }

        // Manejador de eventos para el botón de limpiar búsqueda
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            CargarDatos();
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
                        ventana7 = new Window7(ClienteSeleccionado, true); // Pasa el cliente seleccionado a la ventana7
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
                    MessageBox.Show("No se ha seleccionado ningún cliente.");
                }
            }
        }


        // Método para actualizar los datos del cliente
        private void ActualizarDatosCliente(object sender, EventArgs e)
        {
            // Cargar los datos nuevamente para reflejar el nuevo cliente agregado
            CargarDatos();
        }
    }
}

