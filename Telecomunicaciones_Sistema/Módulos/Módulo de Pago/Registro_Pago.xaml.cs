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
    /// Lógica de interacción para Registro_Pago.xaml
    /// </summary>
    public partial class Registro_Pago : Window
    {
        private Agregar_Pago ventana9;
        // Estructura para representar los pagos
        public static Pagos PagoSeleccionado { get; set; }

        // Constructor de la ventana
        public Registro_Pago()
        {
            InitializeComponent();
            Conn = BD.ObtenerConexion();
            CargarDatos();
            ventana9 = new Agregar_Pago();
        }

        // Estructura para representar los pagos
        public struct Pagos
        {
            public string ID_Pago;
            public string ID_Cliente;
            public string Monto;
            public string ID_Servicio;
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
            Menú frmPr = new Menú(isInicio_Sesión: true);
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
                Agregar_Pago frmAg = new Agregar_Pago();
                frmAg.PagoModificado += (s, args) => CargarDatos(); // Suscribirse al evento PagoModificado
                frmAg.Show();
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
                    ID_Servicio = rowView["ID_Servicio"].ToString(),
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
            Vista_Pago ventana10 = new Vista_Pago();
            ventana10.Show();
            this.Close(); 
        }
    }
}