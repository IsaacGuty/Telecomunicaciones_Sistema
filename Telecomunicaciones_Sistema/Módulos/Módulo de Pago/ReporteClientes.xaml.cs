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
            InitializeComponent();
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

        private void btnRegresar_Click(object sender, RoutedEventArgs e)
        {
            // Regresar a la ventana principal
            Menú frmPr = new Menú(isInicio_Sesión: true);
            frmPr.Show();

            this.Close();
        }

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string mesSeleccionado = ((ComboBoxItem)MesComboBox.SelectedItem)?.Content.ToString();
            string servicioSeleccionado = ((ComboBoxItem)ServicioComboBox.SelectedItem)?.Content.ToString();

            if (string.IsNullOrEmpty(mesSeleccionado) || string.IsNullOrEmpty(servicioSeleccionado))
            {
                MessageBox.Show("Por favor, seleccione un mes y un servicio.");
                return;
            }

            DataTable dt = PagoDAL.ObtenerClientesDeudores(mesSeleccionado, servicioSeleccionado);
            DataGridCD.ItemsSource = dt.DefaultView;
        }

    }
}


