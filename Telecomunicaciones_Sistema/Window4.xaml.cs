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

        public struct Ordenes
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Dirección { get; set; }
            public decimal Teléfono { get; set; }
            public string Servicio { get; set; }
        }

        SqlConnection Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");

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
            this.Hide();

            Window1 frmAn = new Window1();

            frmAn.Show();
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            Window5 formularioD = new Window5();

            formularioD.Show();

            this.Hide();
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
                    txtDirección.Text = rowView["Direccion"].ToString();
                    txtNumT.Text = rowView["Teléfono"].ToString();
                    txtTpServicio.Text = rowView["Apellido"].ToString();
                }
            }
        }
    }
}
