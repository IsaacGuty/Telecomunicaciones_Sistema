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
    /// Lógica de interacción para Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        public Window4()
        {
            InitializeComponent();

            CargarDatos();
        }

        public Ordenes DatosOrden { get; set; }

        SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
        private bool isMainWindow;


        private void CargarDatos()
        {
            try
            {
                Conn.Open();
                string query = "select c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio from Clientes c join Dirección d on d.ID_Dirección = c.ID_Dirección join Pago p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio";
                SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "Ordenes");
                DatGridOT.ItemsSource = dataSet.Tables["Ordenes"].DefaultView;
                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 frmPr = new Window1(isMainWindow: true);
            frmPr.Show();

            if (!isMainWindow)
            {
                this.Close();
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDirección.Clear();
            txtNumT.Clear();
            txtTpServicio.Clear();
            cmbTipoT.Items.Clear();
            cmbNombreE.Items.Clear();
            txtBuscar.Clear();

            if (string.IsNullOrEmpty(txtBuscar.Text))
            {
                CargarDatos();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            DatGridOT.ItemsSource = OrdenDAL.BuscarOrden(txtBuscar.Text);
        }

        private void DatGridOT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridOT.SelectedItem != null)
            {
                DataRowView rowView = DatGridOT.SelectedItem as DataRowView;

                if (rowView != null)
                {
                    txtNombre.Text = rowView["Nombre"].ToString();
                    txtApellido.Text = rowView["Apellido"].ToString();
                    txtDirección.Text = rowView["Dirección"].ToString();
                    txtNumT.Text = rowView["Teléfono"].ToString();
                    txtTpServicio.Text = rowView["Servicio"].ToString();
                }
            }
        }

        private string valorSeleccionadoTipoT;
        private string valorSeleccionadoNombreE;

        private void CmbTipoT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoT.SelectedItem != null)
            {
                ListBoxItem itemSeleccionado = cmbTipoT.SelectedItem as ListBoxItem;
                if (itemSeleccionado != null)
                {
                    valorSeleccionadoTipoT = itemSeleccionado.Content.ToString(); 

                    Window5 ventana5 = new Window5();
                    ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE); 
                }
            }
        }

        private void CmbNombreE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbNombreE.SelectedItem != null)
            {
                ListBoxItem itemSeleccionado = cmbNombreE.SelectedItem as ListBoxItem;
                if (itemSeleccionado != null)
                {
                    valorSeleccionadoNombreE = itemSeleccionado.Content.ToString(); 

                    Window5 ventana5 = new Window5();
                    ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE); 
                }
            }
        }

        private void BtnMostrar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtNombre.Text) || string.IsNullOrEmpty(txtApellido.Text) || string.IsNullOrEmpty(txtDirección.Text) || string.IsNullOrEmpty(txtNumT.Text) || string.IsNullOrEmpty(txtTpServicio.Text) || cmbTipoT.SelectedItem == null || cmbNombreE.SelectedItem == null)
            {
                MessageBox.Show("Por favor, complete todos los campos antes de imprimir.", "Datos Incompletos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Window5 ventana5 = new Window5();

            ventana5.DatosOrden = new Ordenes
            {
                Nombre = txtNombre.Text,
                Apellido = txtApellido.Text,
                Dirección = txtDirección.Text,
                Teléfono = Convert.ToDecimal(txtNumT.Text),
                Servicio = txtTpServicio.Text
            };

            ventana5.ActualizarDatos(valorSeleccionadoTipoT, valorSeleccionadoNombreE);

            ventana5.Show();

            this.Hide();
        }
    }
}
