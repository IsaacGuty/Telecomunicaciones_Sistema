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
    /// Lógica de interacción para Window5.xaml
    /// </summary>
    public partial class Window5 : Window
    {
        public Ordenes DatosOrden { get; set; } 

        public Window5()
        {
            InitializeComponent();

            if (DatosOrden != null)
            {
                txtNombre.Text = DatosOrden.Nombre;
                txtDirección.Text = DatosOrden.Dirección;
                txtNumT.Text = DatosOrden.Teléfono.ToString();
                txtServicio.Text = DatosOrden.Servicio;
            }
        }

        private bool isMainWindow;

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        public void ActualizarDatos(string valorSeleccionadoTipoT, string valorSeleccionadoNombreE)
        {
            if (DatosOrden != null)
            {
                txtNombre.Text = DatosOrden.Nombre + " " + DatosOrden.Apellido;
                txtDirección.Text = DatosOrden.Dirección;
                txtNumT.Text = DatosOrden.Teléfono.ToString();
                txtServicio.Text = DatosOrden.Servicio;
                txtFecha.Text = DateTime.Now.ToShortDateString();

                txtTipoT1.Text = valorSeleccionadoTipoT;
                txtNombreE.Text = valorSeleccionadoNombreE;
            }
        }

        private PrintDialog printDialog;

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Ocultar los botones antes de mostrar la vista previa
            btnRegresar.Visibility = Visibility.Collapsed;
            btnImprimir.Visibility = Visibility.Collapsed;

            // Capturar una representación visual de la ventana
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(this);

            // Configurar el documento a imprimir
            FixedDocument fixedDocument = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = this.ActualWidth;
            fixedPage.Height = this.ActualHeight;
            fixedPage.Children.Add(new Image { Source = renderTargetBitmap });
            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);

            // Crear una nueva instancia de PrintPreviewWindow
            PrintPreviewWindow printPreviewWindow = new PrintPreviewWindow();

            // Configurar el documento y mostrar la ventana de vista previa
            printPreviewWindow.SetDocument(fixedDocument);
            printPreviewWindow.ShowDialog();

            // Restaurar la visibilidad de los botones después de imprimir o cerrar el cuadro de diálogo
            btnRegresar.Visibility = Visibility.Visible;
            btnImprimir.Visibility = Visibility.Visible;
        }
    }
}
