using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para Vista_Pago.xaml
    /// </summary>
    public partial class Vista_Pago : Window
    {
        // Variable estática para indicar si se ha hecho una selección desde la ventana 10
        public static bool SeleccionDesdeVentana10 = false;

        // Variable estática privada que mantendrá la instancia única de Vista_Pago
        private static Vista_Pago _instance;

        // Propiedad estática para acceder a la instancia única de Vista_Pago
        public static Vista_Pago Instance
        {
            get
            {
                // Si la instancia aún no ha sido creada, la crea
                if (_instance == null)
                {
                    _instance = new Vista_Pago();
                }
                // Devuelve la instancia única de Vista_Pago
                return _instance;
            }
        }

        // Constructor de la clase Vista_Pago
        public Vista_Pago()
        {
            // Llama al método InitializeComponent, que inicializa todos los componentes de la interfaz de usuario definidos en el archivo XAML
            InitializeComponent();
        }

        // Manejador del evento SelectionChanged para el DataGrid DatGridP
        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Este método se ejecutará cuando cambie la selección en el DataGrid DatGridP
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Establecer la variable SeleccionDesdeVentana10 antes de abrir la ventana2
            SeleccionDesdeVentana10 = true;

            // Crea una instancia de Registro_Cliente
            Registro_Cliente ventana2 = new Registro_Cliente();

            // Deshabilita los botones de agregar y modificar en ventana2
            ventana2.btnAgregar.IsEnabled = false;
            ventana2.BtnModificar.IsEnabled = false;

            // Mostrar un mensaje para instruir al usuario a seleccionar un cliente
            MessageBox.Show("Por favor seleccione un cliente de la lista en la ventana que se abrirá a continuación.", "Instrucción", MessageBoxButton.OK, MessageBoxImage.Information);

            // Muestra ventana2
            ventana2.ShowDialog();

            // Restablece SeleccionDesdeVentana10 a false después de cerrar ventana2
            SeleccionDesdeVentana10 = false;

            // Verifica si se seleccionó un cliente en ventana2
            if (!string.IsNullOrEmpty(ventana2.ClienteSeleccionado.ID_Cliente))
            {
                // Copia el ID del cliente seleccionado en txtBuscar
                txtBuscar.Text = ventana2.ClienteSeleccionado.ID_Cliente;

                // Llama al método BuscarPagos de PagoDAL y asigna el resultado al DataGrid
                DataTable searchResult = PagoDAL.BuscarCliente(txtBuscar.Text);
                DatGridVP.ItemsSource = searchResult.DefaultView;

                // Si no se encuentran pagos, muestra un mensaje informativo
                if (searchResult.Rows.Count == 0)
                {
                    MessageBox.Show("No se encontraron pagos para el cliente especificado.", "Búsqueda sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                // Si no se seleccionó un cliente, muestra un mensaje al usuario
                MessageBox.Show("No se seleccionó ningún cliente. Por favor, vuelva a la lista y seleccione un cliente.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpiar el campo de búsqueda
            txtBuscar.Foreground = new SolidColorBrush(Colors.Gray); // Restablecer el color del texto a gris
            txtBuscar.Text = "Seleccione un ID de cliente"; // Restablecer el placeholder
            DataView emptyDataView = new DataView(new DataTable()); // Crear un DataView vacío
            DatGridVP.ItemsSource = emptyDataView; // Asignar el DataView vacío al DataGrid
        }


        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de Agregar_Pago
            Registro_Pago Registro_Pago = new Registro_Pago();

            // Cierra la instancia actual de Vista_Pago
            this.Close();

            // Muestra la instancia de Agregar_Pago
            Registro_Pago.Show();
        }

        private void txtBuscar_GotFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el texto del campo 'txtBuscar' es igual a "Seleccione un ID de cliente".
            if (txtBuscar.Text == "Seleccione un ID de cliente")
            {
                // Si el texto es igual, lo establece como una cadena vacía, borrando el texto mostrado.
                txtBuscar.Text = "";

                // Cambia el color del texto del campo 'txtBuscar' a negro para indicar que está listo para la entrada del usuario.
                txtBuscar.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtBuscar_LostFocus(object sender, RoutedEventArgs e)
        {
            // Verifica si el texto del control 'txtBuscar' es nulo, vacío o solo contiene espacios en blanco
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                // Si el texto es nulo, vacío o solo espacios, se establece un texto predeterminado
                txtBuscar.Text = "Seleccione un ID de cliente";

                // Cambia el color del texto a gris para indicar que es un texto de marcador de posición
                txtBuscar.Foreground = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                // Si el texto no está vacío, convierte la primera letra de cada palabra a mayúscula
                txtBuscar.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(txtBuscar.Text.ToLower());
            }
        }
    }
}
