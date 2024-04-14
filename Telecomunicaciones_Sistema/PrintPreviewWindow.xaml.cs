using System;
using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;

namespace Telecomunicaciones_Sistema
{
    public partial class PrintPreviewWindow : Window
    {
        // Variable de diálogo de impresión
        private PrintDialog printDialog;

        // Constructor de la ventana
        public PrintPreviewWindow()
        {
            InitializeComponent(); // Inicializa los componentes de la interfaz de usuario definidos en XAML
            PopulatePrinters(); // Llena la lista de impresoras disponibles
            printDialog = new PrintDialog(); // Crea un nuevo diálogo de impresión

            // Añade las impresoras instaladas al comboBox de selección de impresoras
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(printer);
            }
        }

        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            Ayuda helpWindow = new Ayuda(); // Crea una nueva ventana de ayuda
            helpWindow.ShowDialog(); // Muestra la ventana de ayuda de forma modal
        }

        // Llena la lista de impresoras disponibles en el comboBox de selección de impresoras
        private void PopulatePrinters()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(printer);
            }
        }

        // Establece el documento que se va a mostrar en la vista previa de impresión
        public void SetDocument(FixedDocument document)
        {
            documentViewer.Document = document; // Establece el documento en el visualizador de documentos
            LoadPagesThumbnails(document); // Carga las miniaturas de las páginas del documento
        }

        // Carga las miniaturas de las páginas del documento en la vista previa
        private void LoadPagesThumbnails(FixedDocument document)
        {
            var Pages = new List<PageViewModel>(); // Lista para almacenar las miniaturas de las páginas

            // Itera sobre cada página del documento y crea una miniatura de cada una
            for (int i = 0; i < document.DocumentPaginator.PageCount; i++)
            {
                var page = document.DocumentPaginator.GetPage(i); // Obtiene la página actual
                var bitmap = RenderPageToBitmap(page); // Convierte la página en una imagen bitmap
                Pages.Add(new PageViewModel { Thumbnail = bitmap }); // Añade la miniatura a la lista
            }
        }

        // Convierte una página en un bitmap
        private BitmapImage RenderPageToBitmap(DocumentPage page)
        {
            // Crea un bitmap con el tamaño de la página y la resolución adecuada
            var bitmap = new RenderTargetBitmap((int)page.Size.Width, (int)page.Size.Height, 96, 96, PixelFormats.Default);
            var drawingVisual = new DrawingVisual(); // Crea un objeto visual para dibujar

            // Utiliza un contexto de dibujo para dibujar la página en el bitmap
            using (var context = drawingVisual.RenderOpen())
            {
                // Dibuja un fondo blanco para la página
                context.DrawRectangle(Brushes.White, null, new Rect(new Point(), page.Size));

                // Utiliza un VisualBrush para copiar el contenido de la página en el bitmap
                var visualBrush = new VisualBrush(page.Visual);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), page.Size));
            }

            // Renderiza el dibujo en el bitmap
            bitmap.Render(drawingVisual);

            // Codifica el bitmap como PNG
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            // Guarda el bitmap codificado en un MemoryStream
            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                var thumbnail = new BitmapImage(); // Crea un BitmapImage para la miniatura
                thumbnail.BeginInit();
                thumbnail.StreamSource = memoryStream; // Establece la fuente de datos para la miniatura
                thumbnail.CacheOption = BitmapCacheOption.OnLoad; // Carga la imagen en la memoria
                thumbnail.EndInit();
                return thumbnail; // Devuelve la miniatura como un BitmapImage
            }
        }

        // Función para imprimir el documento en la impresora seleccionada
        public void PrintDocument(FixedDocument fixedDocument)
        {
            // Obtiene la impresora seleccionada del comboBox
            string selectedPrinter = cmbPrinter.SelectedItem as string;

            // Verifica si la impresora es válida
            if (!Validaciones.ValidarImpresora(selectedPrinter))
            {
                return;
            }

            // Crea un servidor de impresión local
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueue printQueue; // Cola de impresión para la impresora seleccionada
            try
            {
                // Obtiene la cola de impresión de la impresora seleccionada
                printQueue = printServer.GetPrintQueue(selectedPrinter);
            }
            catch (PrintQueueException ex)
            {
                // Muestra un mensaje de error si ocurre un problema al obtener la cola de impresión
                MessageBox.Show($"Error al obtener la cola de impresión: {ex.Message}");
                return;
            }

            // Verifica si la cola de impresión es válida
            if (!Validaciones.ValidarPrintQueue(printQueue))
            {
                return;
            }

            // Obtiene el ticket de impresión predeterminado de la impresora
            PrintTicket printTicket = printQueue.DefaultPrintTicket;

            // Configuración de color
            if (cmbColor.SelectedItem != null)
            {
                ComboBoxItem colorItem = (ComboBoxItem)cmbColor.SelectedItem;
                string colorOption = colorItem.Content.ToString();

                // Establece el modo de color según la opción seleccionada (Color o Blanco y Negro)
                if (colorOption == "Blanco y negro")
                {
                    printTicket.OutputColor = OutputColor.Monochrome;
                }
                else
                {
                    printTicket.OutputColor = OutputColor.Color;
                }
            }

            // Configuración de tamaño de papel
            if (cmbPaperSize.SelectedItem != null)
            {
                ComboBoxItem selectedItem = cmbPaperSize.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    string paperSizeOption = selectedItem.Content.ToString();

                    // Establece el tamaño de papel según la opción seleccionada (A4 o Carta)
                    if (paperSizeOption == "A4")
                    {
                        printTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                    }
                    else if (paperSizeOption == "Carta")
                    {
                        printTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.NorthAmericaLetter);
                    }
                }
            }

            // Validación y configuración de número de copias
            if (!Validaciones.ValidarCopias(txtCopies.Text, out int copies))
            {
                return;
            }
            printTicket.CopyCount = copies; // Establece el número de copias según la entrada del usuario

            // Configuración de impresión a una o doble cara
            if (rdbUnaC.IsChecked == true)
            {
                printTicket.Duplexing = Duplexing.OneSided; // Impresión a una cara
            }
            else if (rdbDobleC.IsChecked == true)
            {
                printTicket.Duplexing = Duplexing.TwoSidedLongEdge; // Impresión a doble cara
            }

            // Crea un escritor de documentos XPS para la cola de impresión
            XpsDocumentWriter xpsWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);
            xpsWriter.Write(fixedDocument.DocumentPaginator); // Escribe el documento en la cola de impresión

            // Muestra un mensaje indicando que el documento ha sido enviado a la impresora
            MessageBox.Show("Documento enviado a la impresora.");

            // Muestra el estado de la impresora y el número de trabajos pendientes
            string status = printQueue.QueueStatus.ToString();
            int pendingJobs = printQueue.NumberOfJobs;

            MessageBox.Show($"Estado de la impresora: {status}\nTrabajos pendientes: {pendingJobs}");
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            // Imprime el documento mostrado en la vista previa
            PrintDocument(documentViewer.Document as FixedDocument);
        }

        // Clase para almacenar las miniaturas de las páginas del documento
        public class PageViewModel
        {
            public BitmapImage Thumbnail { get; set; } // Propiedad para almacenar la miniatura de la página
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Cierra la ventana
        }
    }
}

