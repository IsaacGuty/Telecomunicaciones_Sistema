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
    /// Lógica de interacción para Window9.xaml
    /// </summary>
    public partial class Window9 : Window
    {
        public event EventHandler PagoAgregado;

        private List<Pagos> pagos;

        public Pagos NuevoPago { get; private set; }

        private Window3 ventanaP;

        public Window9(Window3 ventanaP)
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            pagos = new List<Pagos>();
            NuevoPago = new Pagos();
            this.ventanaP = ventanaP;
        }

        private SqlConnection Conn;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            int direccion;
            if (int.TryParse(txtDirecciónC.Text, out direccion))
            {
                NuevoPago = new Pagos
                {
                    ID_Cliente = txtIDC.Text,
                    Nombre = txtNombreC.Text,
                    Apellido = txtApellidoC.Text,
                    Dirección = txtDirecciónC.Text,
                    Teléfono = Convert.ToDecimal(txtTeléfonoC.Text),
                    Servicio = txtServicio.Text,
                    Monto = Convert.ToDecimal(txtMonto.Text),
                    MesPagado = txtMesP.Text,
                    Nombre_E = txtNombreE.Text,
                };
            }
            try
            {
                if (!string.IsNullOrEmpty(NuevoPago.ID_Cliente))
                {
                    Conn.Open();
                    SqlCommand cmdInsert = new SqlCommand("INSERT INTO Pago (ID_Cliente, Nombre, Apellido, Dirección, Teléfono, Servicio, Monto, MesPagado, Nombre_E) VALUES (@ID_Cliente, @Nombre, @Apellido, @Dirección, @Teléfono, @Servicio, @Monto, @MesPagado, @Nombre_E)", Conn);
                    cmdInsert.Parameters.AddWithValue("@ID_Cliente", NuevoPago.ID_Cliente);
                    cmdInsert.Parameters.AddWithValue("@Nombre", NuevoPago.Nombre);
                    cmdInsert.Parameters.AddWithValue("@Apellido", NuevoPago.Apellido);
                    cmdInsert.Parameters.AddWithValue("@Dirección", NuevoPago.Dirección);
                    cmdInsert.Parameters.AddWithValue("@Teléfono", NuevoPago.Teléfono);
                    cmdInsert.Parameters.AddWithValue("@Servicio", NuevoPago.Servicio);
                    cmdInsert.Parameters.AddWithValue("@Monto", NuevoPago.Monto);
                    cmdInsert.Parameters.AddWithValue("@MesPagado", NuevoPago.MesPagado);
                    cmdInsert.Parameters.AddWithValue("@Nombre_E", NuevoPago.Nombre_E);
                    cmdInsert.ExecuteNonQuery();
                }
                MessageBox.Show("Pago agregado correctamente.");
                OnPagoAgregado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar el cliente: " + ex.Message);
            }
            OnPagoAgregado();

            this.Close();
        }

        private void OnPagoAgregado()
        {
            PagoAgregado?.Invoke(this, EventArgs.Empty);
        }
    }
}
