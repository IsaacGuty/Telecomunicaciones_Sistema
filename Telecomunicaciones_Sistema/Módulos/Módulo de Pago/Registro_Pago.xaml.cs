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
        // Declaración de una variable privada que almacenará una instancia de la clase Agregar_Pago
        private Agregar_Pago ventana9;

        // Estructura para representar los pagos
        public static Pagos PagoSeleccionado { get; set; }

        // Constructor de la ventana 'Registro_Pago'
        public Registro_Pago()
        {
            // Este método es generado automáticamente y configura la interfaz de usuario.
            InitializeComponent();

            // Obtiene una conexión a la base de datos utilizando el método 'ObtenerConexion' de la clase 'BD'
            Conn = BD.ObtenerConexion();

            // Llama al método 'CargarDatos' para cargar la información necesaria para la ventana
            CargarDatos();

            // Crea una nueva instancia de la clase 'Agregar_Pago' y la asigna a la variable 'ventana9'
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
            // Se utiliza un bloque try-catch para manejar posibles excepciones durante la ejecución del método.
            try
            {
                // Se obtiene un DataTable con todos los pagos desde la base de datos utilizando el método ObtenerTodosPagos de la clase PagoDAL.
                DataTable dataTable = PagoDAL.ObtenerTodosPagos();

                // Se asigna la vista predeterminada del DataTable al DataGrid DatGridP para mostrar los datos en la interfaz de usuario.
                DatGridP.ItemsSource = dataTable.DefaultView;

                // Se establece el DataGrid como solo lectura para que los usuarios no puedan modificar los datos directamente desde el DataGrid.
                DatGridP.IsReadOnly = true; // Establecer el DataGrid como solo lectura
            }
            // Se captura cualquier excepción que ocurra durante la ejecución del bloque try.
            catch (Exception ex)
            {
                // Se muestra un mensaje de error al usuario si ocurre una excepción, incluyendo el mensaje de la excepción.
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        // Método manejador de eventos para el clic en el botón "Regresar".
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva instancia de la ventana principal (Menú) con el parámetro isInicio_Sesión configurado como verdadero.
            Menú frmPr = new Menú(isInicio_Sesión: true);

            // Mostrar la ventana principal.
            frmPr.Show();

            // Cerrar la ventana actual (la ventana desde la que se hizo clic en el botón "Regresar").
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

        // Método que se ejecuta cuando el usuario hace clic en el botón 'Agregar'
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            // Llama al método 'SolicitarInformacionPago' para solicitar información de pago
            SolicitarInformacionPago();
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de la clase Vista_Pago.
            Vista_Pago ventana10 = new Vista_Pago();

            // Muestra la ventana recién creada en la pantalla.
            ventana10.Show();

            // Cierra la ventana actual (la ventana desde la que se hizo clic en el botón).
            this.Close();
        }

        // Método que se ejecuta cuando se hace clic en el botón asociado.
        private void btnReporte_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de la clase ReporteClientes.
            ReporteClientes ventanaRp = new ReporteClientes();

            // Muestra la ventana ReporteClientes al usuario.
            ventanaRp.Show();

            // Cierra la ventana actual.
            this.Close();
        }
    }
}