using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Lógica de interacción para Mostrar_Orden.xaml
    /// </summary>
    public partial class Mostrar_Orden : Window
    {
        // Propiedad pública para almacenar una instancia de la clase Ordenes
        public Ordenes DatosOrden { get; set; }

        // Constructor de la clase Mostrar_Orden
        public Mostrar_Orden()
        {
            // Inicializa los componentes del formulario o ventana, generados por el diseñador
            InitializeComponent();

            // Verifica si la propiedad DatosOrden no es nula antes de intentar acceder a sus propiedades
            if (DatosOrden != null)
            {
                // Asigna el valor combinado de Nombre y Apellido a la propiedad Text del control txtNombre
                txtNombre.Text = DatosOrden.Nombre + " " + DatosOrden.Apellido;

                // Asigna el valor de la propiedad Dirección a la propiedad Text del control txtDirección
                txtDirección.Text = DatosOrden.Dirección;

                // Asigna el valor de la propiedad Teléfono (convertido a cadena) a la propiedad Text del control txtNumT
                txtNumT.Text = DatosOrden.Teléfono.ToString();

                // Asigna el valor de la propiedad Servicio a la propiedad Text del control txtServicio
                txtServicio.Text = DatosOrden.Servicio;

                // Asigna la fecha actual (en formato de fecha corta) a la propiedad Text del control txtFecha
                txtFecha.Text = DateTime.Now.ToShortDateString();
            }
        }

        // Esta variable se utiliza para controlar el estado de la sesión de usuario.
        private bool isInicio_Sesión;

        // Método manejador de eventos para el clic en el botón BtnRegresar.
        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Oculta la ventana actual del formulario en la interfaz de usuario.
            this.Hide();

            // Crea una nueva instancia del formulario Registro_Orden.
            Registro_Orden frmPr = new Registro_Orden();

            // Muestra el nuevo formulario Registro_Orden en la interfaz de usuario.
            frmPr.Show();
        }

        public void ActualizarDatos(string valorSeleccionadoTipoT, string valorSeleccionadoNombreE, string valorSeleccionadoTransporte, string idEmpleado, string idPlaca)
        {
            // Verifica si DatosOrden no es nulo
            if (DatosOrden != null)
            {
                // Actualiza el campo txtNombre con el nombre y apellido de DatosOrden
                txtNombre.Text = DatosOrden.Nombre + " " + DatosOrden.Apellido;

                // Actualiza el campo txtDirección con la dirección de DatosOrden
                txtDirección.Text = DatosOrden.Dirección;

                // Actualiza el campo txtNumT con el número de teléfono de DatosOrden convertido a cadena
                txtNumT.Text = DatosOrden.Teléfono.ToString();

                // Actualiza el campo txtServicio con el servicio de DatosOrden
                txtServicio.Text = DatosOrden.Servicio;

                // Establece la fecha actual en el campo txtFecha
                txtFecha.Text = DateTime.Now.ToShortDateString();

                // Actualiza el campo txtTrabajo con el valor proporcionado para el tipo de trabajo
                txtTrabajo.Text = valorSeleccionadoTipoT;

                // Actualiza el campo txtNombre_E con el valor proporcionado para el nombre del empleado
                txtNombre_E.Text = valorSeleccionadoNombreE;

                // Actualiza el campo txtTransporteM con el valor proporcionado para el transporte
                txtTransporteM.Text = valorSeleccionadoTransporte;

                // Actualiza el campo txtIDE con el ID del empleado proporcionado
                txtIDE.Text = idEmpleado;

                // Actualiza el campo txtIDP con el ID de la placa proporcionado
                txtIDP.Text = idPlaca;
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar los botones y etiquetas antes de mostrar la vista previa
            btnRegresar.Visibility = Visibility.Collapsed; // Hace que el botón 'btnRegresar' no sea visible.
            btnImprimir.Visibility = Visibility.Collapsed; // Hace que el botón 'btnImprimir' no sea visible.

            // Configurar el documento a imprimir
            FixedDocument fixedDocument = new FixedDocument(); // Crea un nuevo documento fijo para impresión.

            // Ajustar el tamaño del contenido (incluyendo el DataGrid completo)
            var visual = new DrawingVisual(); // Crea un objeto DrawingVisual para renderizar el contenido visual.
            using (var context = visual.RenderOpen()) // Abre un contexto de renderizado para el DrawingVisual.
            {
                var bounds = new Rect(0, 0, this.ActualWidth, this.ActualHeight); // Define el área de dibujo basado en el tamaño actual de la ventana.
                context.DrawRectangle(new VisualBrush(this), null, bounds); // Dibuja un rectángulo con un pincel VisualBrush que usa el contenido de la ventana actual.
            }

            var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            // Crea un RenderTargetBitmap con el tamaño de la ventana actual y una resolución de 96 DPI para renderizar la imagen.
            renderTargetBitmap.Render(visual); // Renderiza el DrawingVisual al RenderTargetBitmap.

            FixedPage fixedPage = new FixedPage
            {
                Width = this.ActualWidth, // Establece el ancho de la página fija al ancho actual de la ventana.
                Height = this.ActualHeight, // Establece la altura de la página fija a la altura actual de la ventana.
                Margin = new Thickness(0) // Establece el margen de la página fija a 0.
            };

            Image image = new Image
            {
                Source = renderTargetBitmap, // Establece la fuente de la imagen al RenderTargetBitmap que contiene el contenido visual.
                Stretch = Stretch.Fill, // Ajusta la imagen para llenar el área de la página fija.
                Width = this.ActualWidth, // Establece el ancho de la imagen al ancho actual de la ventana.
                Height = this.ActualHeight // Establece la altura de la imagen a la altura actual de la ventana.
            };

            fixedPage.Children.Add(image); // Agrega la imagen a la página fija.

            var pageContent = new PageContent
            {
                Child = fixedPage // Establece la página fija como el contenido de la página de impresión.
            };

            fixedDocument.Pages.Add(pageContent); // Agrega la página de contenido al documento fijo.

            // Crear una nueva instancia de Imprimir_Pantalla
            Imprimir_Pantalla Imprimir_Pantalla = new Imprimir_Pantalla(); // Crea una nueva instancia de la ventana de vista previa de impresión.

            // Configurar el documento y mostrar la ventana de vista previa
            Imprimir_Pantalla.SetDocument(fixedDocument); // Configura el documento fijo en la ventana de vista previa de impresión.
            Imprimir_Pantalla.ShowDialog(); // Muestra la ventana de vista previa como un cuadro de diálogo modal.

            // Restaurar la visibilidad de los botones y etiquetas después de imprimir o cerrar el cuadro de diálogo
            btnRegresar.Visibility = Visibility.Visible; // Hace que el botón 'btnRegresar' vuelva a ser visible.
            btnImprimir.Visibility = Visibility.Visible; // Hace que el botón 'btnImprimir' vuelva a ser visible.
        }
    }
}
