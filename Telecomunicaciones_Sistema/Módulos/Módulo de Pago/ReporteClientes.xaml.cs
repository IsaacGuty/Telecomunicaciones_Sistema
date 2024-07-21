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
using System.Diagnostics;

namespace Telecomunicaciones_Sistema
{
    /// <summary>
    /// Lógica de interacción para ReporteClientes.xaml
    /// </summary>
    public partial class ReporteClientes : Window
    {
        public ReporteClientes()
        {
            // Llama al método para inicializar los componentes del formulario.
            InitializeComponent();

            // Llama al método para cargar los datos necesarios para el reporte.
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                DataTable dataTable = PagoDAL.CargarServicios(); // Obtener datos de clientes desde la base de datos
                DataGridCD.ItemsSource = dataTable.DefaultView; // Mostrar los datos en el DataGrid
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message); // Mostrar mensaje de error en caso de fallo
            }
        }

        // Este es el manejador de eventos para el clic del botón "btnRegresar".
        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Oculta la ventana actual.
            this.Hide();

            // Crea una nueva instancia de la ventana "Registro_Pago".
            Registro_Pago frmP = new Registro_Pago();

            // Muestra la ventana "Registro_Pago" al usuario.
            frmP.Show();
        }

        // Evento que se ejecuta al hacer clic en el botón 'Buscar'.
        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            // Obtiene el mes seleccionado en el ComboBox 'MesComboBox'.
            string mesSeleccionado = ((ComboBoxItem)MesComboBox.SelectedItem)?.Content.ToString();

            // Obtiene el servicio seleccionado en el ComboBox 'ServicioComboBox'.
            string servicioSeleccionado = ((ComboBoxItem)ServicioComboBox.SelectedItem)?.Content.ToString();

            // Verifica si el mes seleccionado o el servicio seleccionado están vacíos o nulos.
            if (string.IsNullOrEmpty(mesSeleccionado) || string.IsNullOrEmpty(servicioSeleccionado))
            {
                // Muestra un mensaje de advertencia indicando que se deben seleccionar un mes y un servicio.
                MessageBox.Show("Por favor, seleccione un mes y un servicio.");

                // Sale del método sin realizar ninguna acción adicional.
                return;
            }

            // Llama al método 'ObtenerClientesDeudores' de la clase 'PagoDAL' para obtener los datos de clientes deudores
            // basados en el mes y el servicio seleccionados.
            DataTable dt = PagoDAL.ObtenerClientesDeudores(mesSeleccionado, servicioSeleccionado);

            // Establece la fuente de datos del DataGrid 'DataGridCD' a la vista del DataTable obtenido.
            DataGridCD.ItemsSource = dt.DefaultView;
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Verifica si el usuario ha seleccionado un mes y un servicio en los ComboBox.
            if (MesComboBox.SelectedItem == null || ServicioComboBox.SelectedItem == null)
            {
                MessageBox.Show("Debe seleccionar un mes y un servicio.");
                return;
            }

            // Oculta los botones y etiquetas para evitar que interfieran con la vista previa de impresión.
            btnBuscar.Visibility = Visibility.Collapsed;
            btnRegresar.Visibility = Visibility.Collapsed;
            btnImprimir.Visibility = Visibility.Collapsed;

            // Crea un nuevo documento fijo para la impresión.
            FixedDocument fixedDocument = new FixedDocument();

            // Configura un DrawingVisual para renderizar la vista actual en un RenderTargetBitmap.
            var visual = new DrawingVisual();
            using (var context = visual.RenderOpen())
            {
                // Define los límites del rectángulo para el contenido a renderizar.
                var bounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight);
                // Dibuja el contenido de la vista actual dentro del DrawingVisual.
                context.DrawRectangle(new VisualBrush(this), null, bounds);
            }

            // Crea un RenderTargetBitmap para capturar la vista renderizada.
            var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            // Renderiza el DrawingVisual en el RenderTargetBitmap.
            renderTargetBitmap.Render(visual);

            // Crea una nueva página fija para el documento de impresión.
            FixedPage fixedPage = new FixedPage
            {
                Width = this.ActualWidth,
                Height = this.ActualHeight,
                Margin = new Thickness(0)
            };

            // Crea una imagen a partir del RenderTargetBitmap.
            Image image = new Image
            {
                Source = renderTargetBitmap,
                Stretch = Stretch.Fill,
                Width = this.ActualWidth,
                Height = this.ActualHeight
            };

            // Agrega la imagen a la página fija.
            fixedPage.Children.Add(image);
            // Crea el contenido de la página para el documento de impresión.
            var pageContent = new PageContent
            {
                Child = fixedPage
            };
            // Agrega la página al documento fijo.
            fixedDocument.Pages.Add(pageContent);

            // Crea una nueva instancia de la ventana de vista previa de impresión.
            Imprimir_Pantalla Imprimir_Pantalla = new Imprimir_Pantalla();

            // Configura el documento para la vista previa de impresión y muestra la ventana de vista previa.
            Imprimir_Pantalla.SetDocument(fixedDocument);
            Imprimir_Pantalla.ShowDialog();

            // Restaura la visibilidad de los botones y etiquetas después de que se haya cerrado la vista previa de impresión.
            btnBuscar.Visibility = Visibility.Visible;
            btnRegresar.Visibility = Visibility.Visible;
            btnImprimir.Visibility = Visibility.Visible;
        }
    }
}


