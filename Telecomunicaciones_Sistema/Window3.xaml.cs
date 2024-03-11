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
    /// Lógica de interacción para Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        // Estructura para representar los pagos
        public static Pagos PagoSeleccionado { get; set; }

        // Constructor de la ventana
        public Window3()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            CargarDatos();
        }

        // Clase para el diálogo de nuevo pago
        public partial class NuevoPagoDialog : Window
        {
            // Propiedades para los datos del nuevo pago
            public string ID_Cliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Dirección { get; set; }
            public string Teléfono { get; set; }
            public string Servicio { get; set; }
            public string Monto { get; set; }
            public string MesPagado { get; set; }
            public string Nombre_E { get; set; }
        }

        // Estructura para representar los pagos
        public struct Pagos
        {
            public string ID_Cliente;
            public string Nombre;
            public string Apellido;
            public string Dirección;
            public string Teléfono;
            public string Servicio;
            public string Monto;
            public string MesPagado;
            public string Nombre_E;
        }

        // Conexión a la base de datos y variable de control para la ventana principal
        private SqlConnection Conn;
        private bool isMainWindow;

        // Método para cargar los datos de los pagos desde la base de datos
        public void CargarDatos()
        {
            try
            {
                DataTable dataTable = PagoDAL.ObtenerTodosPagos();
                DatGridP.ItemsSource = dataTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Manejador del evento click del botón "Regresar"
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }

        // Método para solicitar la información de un nuevo pago
        private void SolicitarInformacionPago()
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo pago.", "Nuevo Pago", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Window9 frmAg = new Window9();
                frmAg.Closed += (s, args) => CargarDatos();
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            DatGridP.ItemsSource = ClienteDAL.BuscarCliente(txtBuscar.Text).DefaultView;
        }

        // Manejador del evento SelectionChanged del DataGrid
        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtiene y guarda la información del pago seleccionado en la estructura Pagos
            if (DatGridP.SelectedItem != null && DatGridP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridP.SelectedItem as DataRowView;

                PagoSeleccionado = new Pagos
                {
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    Nombre = rowView["Nombre"].ToString(),
                    Apellido = rowView["Apellido"].ToString(),
                    Dirección = rowView["Dirección"].ToString(),
                    Teléfono = rowView["Teléfono"].ToString(),
                    Servicio = rowView["Servicio"].ToString(),
                    Monto = rowView["Monto"].ToString(),
                    MesPagado = rowView["Mes_Pagado"].ToString(),
                    Nombre_E = rowView["Nombre_E"].ToString()
                };
            }
            else
            {
                PagoSeleccionado = default(Pagos);
            }
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del pago.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Abre la ventana de modificación de pago si se seleccionó un pago
                if (result == MessageBoxResult.OK && !PagoSeleccionado.Equals(default(Pagos)))
                {
                    Window9 frmMd = new Window9(PagoSeleccionado);
                    frmMd.PagoModificado += ActualizarDatosPago;
                    frmMd.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún cliente.");
                }
            }
        }

        // Método para actualizar los datos de los pagos después de una modificación
        private void ActualizarDatosPago(object sender, EventArgs e)
        {
            CargarDatos();
        }
    }
}
