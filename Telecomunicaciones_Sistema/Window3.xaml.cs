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
    /// Lógica de interacción para Window3.xaml
    /// </summary>
    public partial class Window3 : Window
    {
        public static Pagos PagoSeleccionado { get; set; }

        public Window3()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            CargarDatos();
        }

        public partial class NuevoPagoDialog : Window
        {
            public string ID_Cliente { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Dirección { get; set; }
            public string Teléfono { get; set; }
            public string Servicio { get; set; }
            public string Monto { get; set; }
            public string MesPagado { get; set; }
            public string Nombre_E { get; set; }
        }

        public struct Pagos
        {
            public string ID_Cliente;
            public string Nombre;
            public string Apellido;
            public string Dirección;
            public string Teléfono;
            public string Servicio;
            public string Monto;
            public string MesPagado;
            public string Nombre_E;
        }

        private SqlConnection Conn;
        private bool isMainWindow;

        public void CargarDatos()
        {
            try
            {
                Conn.Open();
                string query = "select c.ID_Cliente, c.Nombre, c.Apellido, d.Dirección, c.Teléfono, s.Servicio, p.Monto, p.Mes_Pagado, e.Nombre_E from Clientes c join Pago p on p.ID_Cliente = c.ID_Cliente join Servicios s on s.ID_Servicio = p.ID_TpServicio join Empleados e on e.ID_Empleado = p.ID_Empleado join Dirección d on d.ID_Dirección = c.ID_Dirección";
                SqlDataAdapter adapter = new SqlDataAdapter(query, Conn);
                DataSet dataSet = new DataSet();
                adapter.Fill(dataSet, "Pago");
                DatGridP.ItemsSource = dataSet.Tables["Pago"].DefaultView;
                Conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
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

       /* private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            SolicitarInformacionPago();
        }*/

        private void SolicitarInformacionPago()
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la información del nuevo pago.", "Nuevo Pago", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                Window9 frmAg = new Window9();
                frmAg.Closed += (s, args) => CargarDatos();
                frmAg.Show();
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            CargarDatos();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            DatGridP.ItemsSource = ClienteDAL.BuscarCliente(txtBuscar.Text);
        }

        private void DatGridP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DatGridP.SelectedItem != null && DatGridP.SelectedItem is DataRowView)
            {
                DataRowView rowView = DatGridP.SelectedItem as DataRowView;

                PagoSeleccionado = new Pagos
                {
                    ID_Cliente = rowView["ID_Cliente"].ToString(),
                    Nombre = rowView["Nombre"].ToString(),
                    Apellido = rowView["Apellido"].ToString(),
                    Dirección = rowView["Dirección"].ToString(),
                    Teléfono = rowView["Teléfono"].ToString(),
                    Servicio = rowView["Servicio"].ToString(),
                    Monto = rowView["Monto"].ToString(),
                    MesPagado = rowView["Mes_Pagado"].ToString(),
                    Nombre_E = rowView["Nombre_E"].ToString()
                };
            }
            else
            {
                PagoSeleccionado = default(Pagos);
            }
        }

        private void BtnModificar_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Por favor, ingrese la modificación del pago.", "Modificación", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                if (result == MessageBoxResult.OK && !PagoSeleccionado.Equals(default(Pagos)))
                {
                    Window9 frmMd = new Window9(PagoSeleccionado);
                    frmMd.PagoModificado += ActualizarDatosPago;
                    frmMd.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No se ha seleccionado ningún cliente.");
                }
            }
        }

        private void ActualizarDatosPago(object sender, EventArgs e)
        {
            CargarDatos();
        }
    }
}
