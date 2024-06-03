using System;
using System.Collections.Generic;
using System.Data;
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
    /// Lógica de interacción para Window10.xaml
    /// </summary>
    public partial class Window10 : Window
    {
        public static bool SeleccionDesdeVentana10 = false;

        private static Window10 _instance;

        // Propiedad estática para acceder a la instancia única de Window10
        public static Window10 Instance
        {
            get
            {
                // Si la instancia aún no ha sido creada, la crea
                if (_instance == null)
                {
                    _instance = new Window10();
                }
                return _instance;
            }
        }

        public Window10()
        {
            InitializeComponent();
        }

        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        // Constructor que acepta el texto a buscar
        public Window10(string textoBusqueda) : this()
        {
            txtBuscar.Text = textoBusqueda; // Establece el texto de búsqueda en el TextBox

            // Llama al método BuscarPagos de PagoDAL y asigna el resultado al DataGrid
            DatGridVP.ItemsSource = PagoDAL.BuscarCliente(textoBusqueda).DefaultView;
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Establecer la variable SeleccionDesdeVentana10 antes de abrir la ventana2
            SeleccionDesdeVentana10 = true;

            // Crea una instancia de Window2
            Window2 ventana2 = new Window2();

            // Deshabilita los botones de agregar y modificar en ventana2
            ventana2.btnAgregar.IsEnabled = false;
            ventana2.BtnModificar.IsEnabled = false;

            // Mostrar un mensaje para instruir al usuario a seleccionar un cliente
            MessageBox.Show("Por favor, seleccione un cliente de la lista en la ventana que se abrirá a continuación.", "Instrucción", MessageBoxButton.OK, MessageBoxImage.Information);

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

            // Crear un DataView vacío
            DataView emptyDataView = new DataView(new DataTable());

            // Asignar el DataView vacío al DataGrid
            DatGridVP.ItemsSource = emptyDataView;
        }


        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Crea una nueva instancia de Window9
            Window3 window3 = new Window3();

            // Cierra la instancia actual de Window10
            this.Close();

            // Muestra la instancia de Window9
            window3.Show();
        }
    }
}
