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
        private Window9 ventana9;
        // Estructura para representar los pagos
        public static Pagos PagoSeleccionado { get; set; }

        // Constructor de la ventana
        public Window3()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion();
            CargarDatos();
            ventana9 = new Window9();
        }

        // Clase para el diálogo de nuevo pago
        public partial class NuevoPagoDialog : Window
        {
            // Propiedades para los datos del nuevo pago
            public string ID_Pago { get; set; }
            public string ID_Cliente { get; set; }
            public decimal Monto { get; set; }
            public string ID_TpServicio { get; set; }
            public string MesPagado { get; set; }
            public decimal Fecha { get; set; }
            public string ID_Empleado { get; set; }
        }

        // Estructura para representar los pagos
        public struct Pagos
        {
            public string ID_Pago;
            public string ID_Cliente;
            public string Monto;
            public string ID_TpServicio;
            public string MesPagado;
            public string Fecha;
            public string ID_Empleado;
        }

        // Conexión a la base de datos y variable de control para la ventana principal
        private SqlConnection Conn;

        // Método para cargar los datos de los pagos desde la base de datos
        public void CargarDatos()
        {
            try
            {
                DataTable dataTable = PagoDAL.ObtenerTodosPagos();
                DatGridP.ItemsSource = dataTable.DefaultView;
                DatGridP.IsReadOnly = true; // Establecer el DataGrid como solo lectura
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Regresar a la ventana principal
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            this.Close();
        }

        // Método para solicitar la información de un nuevo pago
        private void SolicitarInformacionPago()
        {
            // Mostrar un diálogo para ingresar información de un nuevo pago
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo pago.", "Nuevo Pago", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Abrir la ventana para agregar un nuevo pago
                Window9 frmAg = new Window9();
                frmAg.PagoModificado += (s, args) => CargarDatos(); // Suscribirse al evento PagoModificado
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Limpiar el cuadro de búsqueda y recargar los datos
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Verificar si se ha ingresado un ID de pago
            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                // Mostrar un mensaje informando al usuario que debe ingresar un ID de pago
                MessageBox.Show("Debe ingresar un ID de pago para realizar la búsqueda.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Detener la ejecución de la función si no se ha ingresado un ID de pago
            }

            // Buscar pagos según el texto ingresado en el campo de búsqueda
            DataTable dataTable = PagoDAL.BuscarPago(txtBuscar.Text);
            DataView dataView = new DataView(dataTable);
            DatGridP.ItemsSource = dataView;

            // Verificar si el DataTable está vacío
            if (dataTable.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron pagos que coincidan con la búsqueda.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Manejador del evento SelectionChanged del DataGrid
        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Obtener y guardar la información del pago seleccionado en la estructura Pagos
            if (DatGridP.SelectedItem != null && DatGridP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridP.SelectedItem as DataRowView;

                PagoSeleccionado = new Pagos
                {
                    ID_Pago = rowView["ID_Pago"].ToString(),
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    ID_TpServicio = rowView["ID_TpServicio"].ToString(),
                    Monto = rowView["Monto"].ToString(),
                    MesPagado = rowView["Mes_Pagado"].ToString(),
                    Fecha = rowView["Fecha"].ToString(),
                    ID_Empleado = rowView["ID_Empleado"].ToString()
                };
            }
            else
            {
                PagoSeleccionado = default(Pagos);
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionPago();
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            // Abre la ventana 10
            Window10 ventana10 = new Window10();
            ventana10.Show();
            this.Close(); // Opcional: Cierra la ventana actual si es necesario
        }
    }
}