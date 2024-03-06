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
        //public event EventHandler PagoAgregado;

        public event EventHandler PagoModificado;

        private List<Pagos> pagos;

        public Pagos NuevoPago { get; private set; }

        public Window9()
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            pagos = new List<Pagos>();
        }

        public Window9(Window3.Pagos pagoSeleccionado)
        {
            InitializeComponent();
            Conn = new SqlConnection("Data source = DESKTOP-KIBLMD6\\SQLEXPRESS; Initial catalog = TelecomunicacionesBD; Integrated security = true");
            pagos = new List<Pagos>();
            this.pagoSeleccionado = pagoSeleccionado;
            MostrarDetallesPago();
        }

        private SqlConnection Conn;
        private Window3.Pagos pagoSeleccionado;

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
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
            try
            {
                Conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pago WHERE ID_Cliente = @ID_Cliente", Conn);
                cmd.Parameters.AddWithValue("@ID_Cliente", NuevoPago.ID_Cliente);
                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    cmd = new SqlCommand("UPDATE Pago SET Mes_Pagado = @MesPagado WHERE ID_Cliente = @ID_Cliente", Conn);
                }

                cmd.Parameters.AddWithValue("@ID_Cliente", NuevoPago.ID_Cliente);
                cmd.Parameters.AddWithValue("@MesPagado", NuevoPago.MesPagado);
                cmd.ExecuteNonQuery();

                SqlCommand updateServiceCmd = new SqlCommand("UPDATE Servicios SET Servicio = @Servicio WHERE ID_Servicio = (SELECT ID_TpServicio FROM Pago WHERE ID_Cliente = @ID_Cliente)", Conn);
                updateServiceCmd.Parameters.AddWithValue("@ID_Cliente", NuevoPago.ID_Cliente);
                updateServiceCmd.Parameters.AddWithValue("@Servicio", NuevoPago.Servicio);
                updateServiceCmd.ExecuteNonQuery();

                Conn.Close();

                if (count > 0)
                {
                    MessageBox.Show("Pago modificado correctamente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar el pago: " + ex.Message);
            }
            finally
            {
                Conn.Close();
            }

            OnPagoModificado();

            this.Close();
        }

       /* private void OnPagoAgregado()
        {
            PagoAgregado?.Invoke(this, EventArgs.Empty);
        }*/

        private void OnPagoModificado()
        {
            PagoModificado?.Invoke(this, EventArgs.Empty);
        }

        private void MostrarDetallesPago()
        {
            txtIDC.Text = pagoSeleccionado.ID_Cliente;
            txtNombreC.Text = pagoSeleccionado.Nombre;
            txtApellidoC.Text = pagoSeleccionado.Apellido;
            txtDirecciónC.Text = pagoSeleccionado.Dirección;
            txtTeléfonoC.Text = pagoSeleccionado.Teléfono;
            txtServicio.Text = pagoSeleccionado.Servicio;
            txtMonto.Text = pagoSeleccionado.Monto;
            txtMesP.Text = pagoSeleccionado.MesPagado;
            txtNombreE.Text = pagoSeleccionado.Nombre_E;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            Window3 frmPr = new Window3();
        }
    }
}
