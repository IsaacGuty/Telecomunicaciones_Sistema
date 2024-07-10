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
        public Ordenes DatosOrden { get; set; }

        public Mostrar_Orden()
        {
            InitializeComponent();

            txtNombre.IsReadOnly = true;
            txtDirección.IsReadOnly = true;
            txtNumT.IsReadOnly = true;
            txtServicio.IsReadOnly = true;
            txtFecha.IsReadOnly = true;
            txtTrabajo.IsReadOnly = true;
            txtIDE.IsReadOnly = true;
            txtNombre_E.IsReadOnly = true;

            // Comprobar si DatosOrden no es nulo antes de asignar los valores
            if (DatosOrden != null)
            {
                txtNombre.Text = DatosOrden.Nombre + " " + DatosOrden.Apellido;
                txtDirección.Text = DatosOrden.Dirección;
                txtNumT.Text = DatosOrden.Teléfono.ToString();
                txtServicio.Text = DatosOrden.Servicio;
                txtFecha.Text = DateTime.Now.ToShortDateString();
            }
        }

        private bool isInicio_Sesión;

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            if (!isInicio_Sesión)
            {
                this.Close();
            }
        }

        public void ActualizarDatos(string valorSeleccionadoTipoT, string valorSeleccionadoNombreE, string idEmpleado)
        {
            if (DatosOrden != null)
            {
                txtNombre.Text = DatosOrden.Nombre + " " + DatosOrden.Apellido;
                txtDirección.Text = DatosOrden.Dirección;
                txtNumT.Text = DatosOrden.Teléfono.ToString();
                txtServicio.Text = DatosOrden.Servicio;
                txtFecha.Text = DateTime.Now.ToShortDateString();

                txtTrabajo.Text = valorSeleccionadoTipoT;
                txtNombre_E.Text = valorSeleccionadoNombreE;
                txtIDE.Text = idEmpleado;
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar los botones y etiquetas antes de mostrar la vista previa
            btnRegresar.Visibility = Visibility.Collapsed;
            btnImprimir.Visibility = Visibility.Collapsed;

            // Capturar una representación visual de la ventana
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);

            // Configurar el documento a imprimir
            FixedDocument fixedDocument = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            // Ajustar el ancho y alto al de la ventana principal
            fixedPage.Width = this.ActualWidth;
            fixedPage.Height = this.ActualHeight;

            // Establecer márgenes a cero para eliminar espacios en blanco alrededor del contenido
            fixedPage.Margin = new Thickness(0);

            // Agregar la imagen al contenido de la página
            Image image = new Image
            {
                Source = renderTargetBitmap,
                Stretch = Stretch.Fill // Ajustar la imagen para llenar toda la página
            };

            // Ajustar el tamaño de la imagen para que se ajuste a la página fija
            image.Width = this.ActualWidth;
            image.Height = this.ActualHeight;

            fixedPage.Children.Add(image);
            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            // Crear una nueva instancia de Imprimir_Pantalla
            Imprimir_Pantalla Imprimir_Pantalla = new Imprimir_Pantalla();

            // Configurar el documento y mostrar la ventana de vista previa
            Imprimir_Pantalla.SetDocument(fixedDocument);
            Imprimir_Pantalla.ShowDialog();

            // Restaurar la visibilidad de los botones y etiquetas después de imprimir o cerrar el cuadro de diálogo
            btnRegresar.Visibility = Visibility.Visible;
            btnImprimir.Visibility = Visibility.Visible;
        }
    }
}
