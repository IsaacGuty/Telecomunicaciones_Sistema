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
    /// Lógica de interacción para Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public static Clientes ClienteSeleccionado { get; set; }

        public Window2()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            CargarDatos();
        }

        public partial class NuevoClienteDialog : Window
        {
            public string ID_Cliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Teléfono { get; set; }
            public string Correo { get; set; }
            public string ID_Dirección { get; set; }
        }

        public struct Clientes
        {
            public string ID_Cliente;
            public string Nombre;
            public string Apellido;
            public string Teléfono;
            public string Correo;
            public string ID_Dirección;
        }

        private SqlConnection Conn;

        private void CargarDatos()
        {
            try
            {
                    Conn.Open();
                    string query = "SELECT * FROM Clientes";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Clientes");
                    DatGridRC.ItemsSource = dataSet.Tables["Clientes"].DefaultView;
                    Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window1 frmPr = new Window1();

            frmPr.Show();
        }

        private void DatGridRC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridRC.SelectedItem != null && DatGridRC.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridRC.SelectedItem as DataRowView;

                ClienteSeleccionado = new Clientes
                {
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    Nombre = rowView["Nombre"].ToString(),
                    Apellido = rowView["Apellido"].ToString(),
                    Teléfono = rowView["Teléfono"].ToString(),
                    Correo = rowView["Correo"].ToString(),
                    ID_Dirección = rowView["ID_Dirección"].ToString()
                };
            }
            else
            {
                ClienteSeleccionado = default(Clientes);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionCliente();
        }

        private void SolicitarInformacionCliente()
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo cliente.", "Nuevo Cliente", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Window7 frmAg = new Window7();
                frmAg.Closed += (s, args) => CargarDatos();
                frmAg.Show();
            }
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            DatGridRC.ItemsSource = ClienteDAL.BuscarCliente(txtBuscar.Text);
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del cliente.", "Modificación", MessageBoxButton.OKCancel);

            if (!ClienteSeleccionado.Equals(default(Clientes)))
            {
                Window7 frmMd = new Window7(ClienteSeleccionado);
                frmMd.ClienteModificado += ActualizarDatosCliente;
                frmMd.ShowDialog(); 
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ningún cliente.");
            }
        }

        private void ActualizarDatosCliente(object sender, EventArgs e)
        {
            CargarDatos(); 
        }
    }
}
