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
        private PrintDialog printDialog;

        public PrintPreviewWindow()
        {
            InitializeComponent();
            PopulatePrinters();
            printDialog = new PrintDialog();

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(printer);
            }
        }

        private void btnAyuda_Click(object sender, RoutedEventArgs e)
        {
            Ayuda helpWindow = new Ayuda();
            helpWindow.ShowDialog();
        }

        private void PopulatePrinters()
        {
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbPrinter.Items.Add(printer);
            }
        }

        public void SetDocument(FixedDocument document)
        {
            documentViewer.Document = document;
            LoadPagesThumbnails(document);
        }

        private void LoadPagesThumbnails(FixedDocument document)
        {
            var Pages = new List<PageViewModel>();

            for (int i = 0; i < document.DocumentPaginator.PageCount; i++)
            {
                var page = document.DocumentPaginator.GetPage(i);
                var bitmap = RenderPageToBitmap(page);
                Pages.Add(new PageViewModel { Thumbnail = bitmap });
            }

        }

        private BitmapImage RenderPageToBitmap(DocumentPage page)
        {
            var bitmap = new RenderTargetBitmap((int)page.Size.Width, (int)page.Size.Height, 96, 96, PixelFormats.Default);
            var drawingVisual = new DrawingVisual();

            using (var context = drawingVisual.RenderOpen())
            {
                context.DrawRectangle(Brushes.White, null, new Rect(new Point(), page.Size));

                var visualBrush = new VisualBrush(page.Visual);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), page.Size));
            }

            bitmap.Render(drawingVisual);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                memoryStream.Position = 0;
                var thumbnail = new BitmapImage();
                thumbnail.BeginInit();
                thumbnail.StreamSource = memoryStream;
                thumbnail.CacheOption = BitmapCacheOption.OnLoad;
                thumbnail.EndInit();
                return thumbnail;
            }
        }

        public void PrintDocument(FixedDocument fixedDocument)
        {
            string selectedPrinter = cmbPrinter.SelectedItem as string;
            if (selectedPrinter == null)
            {
                MessageBox.Show("Por favor, seleccione una impresora.");
                return;
            }

            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueue printQueue = null;
            try
            {
                printQueue = printServer.GetPrintQueue(selectedPrinter);
            }
            catch (PrintQueueException ex)
            {
                MessageBox.Show($"Error al obtener la cola de impresión: {ex.Message}");
                return;
            }

            if (printQueue.IsOffline)
            {
                MessageBox.Show("La impresora está desconectada o fuera de línea.");
                return;
            }

            PrintTicket printTicket = printQueue.DefaultPrintTicket;

            if (cmbColor.SelectedItem != null)
            {
                ComboBoxItem colorItem = (ComboBoxItem)cmbColor.SelectedItem;
                string colorOption = colorItem.Content.ToString();

                if (colorOption == "Blanco y negro")
                {
                    printTicket.OutputColor = OutputColor.Monochrome;
                }
                else
                {
                    printTicket.OutputColor = OutputColor.Color;
                }
            }

            if (cmbPaperSize.SelectedItem != null)
            {
                ComboBoxItem selectedItem = cmbPaperSize.SelectedItem as ComboBoxItem;

                if (selectedItem != null)
                {
                    string paperSizeOption = selectedItem.Content.ToString();

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

            if (!string.IsNullOrEmpty(txtCopies.Text) && int.TryParse(txtCopies.Text, out int copies) && copies > 0)
            {
                printTicket.CopyCount = copies;
            }

            if (rdbUnaC.IsChecked == true)
            {
                printTicket.Duplexing = Duplexing.OneSided;
            }
            else if (rdbDobleC.IsChecked == true)
            {
                printTicket.Duplexing = Duplexing.TwoSidedLongEdge;
            }

            XpsDocumentWriter xpsWriter = PrintQueue.CreateXpsDocumentWriter(printQueue);

            xpsWriter.Write(fixedDocument.DocumentPaginator);

            MessageBox.Show("Documento enviado a la impresora.");

            string status = printQueue.QueueStatus.ToString();
            int pendingJobs = printQueue.NumberOfJobs;

            MessageBox.Show($"Estado de la impresora: {status}\nTrabajos pendientes: {pendingJobs}");
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument(documentViewer.Document as FixedDocument);
        }

        public class PageViewModel
        {
            public BitmapImage Thumbnail { get; set; }
        }
    }
}
